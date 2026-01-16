namespace Blockpit.Tests
{
    using Configuration;
    using Data;
    using FluentMigrator.Runner;
    using LinqToDB;
    using LinqToDB.Async;
    using log4net;
    using log4net.Config;
    using Mediator.Commands.Blockpit.Mediator.Commands;
    using Mediator.Exceptions;
    using Mediator.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Migrations;
    using Observability;

    public class DatabaseStoreBlockHandler
    {
        private readonly CentralCounterService _centralCounterService;
        private readonly ILog _log;
        private readonly Settings _settings;
        private readonly CancellationToken _token;

        public DatabaseStoreBlockHandler()
        {
            var configFile = new FileInfo("log4net.config");
            XmlConfigurator.Configure(configFile);
            _log = LogManager.GetLogger(typeof(ILog));

            _settings = new Settings();
            _centralCounterService = new CentralCounterService();
            _token = CancellationToken.None;

            if (File.Exists("blockpit.db"))
            {
                File.Delete("blockpit.db");
            }

            var services = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString(_settings.ConnectionString)
                    .ScanIn(typeof(Migration20260113125200).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

            var provider = services.BuildServiceProvider();

            using var scope = provider.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }

        [Fact]
        public async Task ValidateBlockTickAndIdempotency()
        {
            var blockTick = new BlockTick("BTC",
                "BTC.main",
                932477,
                "00000000000000000000ac1fee616ab05bd28f5a92bc5e6fb81488f403b0c2a2",
                DateTime.Parse("2026-01-16T06:14:55.1601082Z"),
                "https://api.blockcypher.com/v1/btc/main/blocks/00000000000000000000ac1fee616ab05bd28f5a92bc5e6fb81488f403b0c2a2",
                "0000000000000000000121e19b8b4b75f15b272f62a579625554da68e8bf8906",
                "https://api.blockcypher.com/v1/btc/main/blocks/0000000000000000000121e19b8b4b75f15b272f62a579625554da68e8bf8906",
                324,
                2912,
                929868,
                "00000000000000000000ce3e0f27d6e6ce5c8bd585126b6968bfb6c3a1cc3a7c"
            );

            blockTick.SetUtxoFees(3391, 1677, 1263);
            blockTick.SetGasFees(12521440124, 5322447872, 726874753, 1649891251, 731647652, 146427779, 83063688);

            var handler = new Mediator.Handlers.DatabaseStoreBlockHandler(_log, _settings, _centralCounterService);
            var command = new ProcessBlockCommand(blockTick);
            await handler.Handle(command, _token);

            var options = new DataOptions().UseSQLite(_settings.ConnectionString);
            var dbContext = new DbContext(options);

            var blockTickInDatabase = await dbContext
                .BlockTick.Where(w => w.Hash == blockTick.Hash
                                      && w.LastForkHash == blockTick.LastForkHash
                                      && w.LatestUrl == blockTick.LatestUrl
                                      && w.Hash == blockTick.Hash
                                      && w.Name == blockTick.Name
                                      && w.PreviousHash == blockTick.PreviousHash
                                      && w.PreviousUrl == blockTick.PreviousUrl
                                      && w.Symbol == blockTick.Symbol
                                      && w.Height == blockTick.Height
                                      && w.LastForkHeight == blockTick.LastForkHeight
                                      && w.PeerCount == blockTick.PeerCount
                                      && w.Time == blockTick.Time
                                      && w.UnconfirmedCount == blockTick.UnconfirmedCount
                                      ).FirstOrDefaultAsync(_token);

            Assert.True(blockTickInDatabase != null, "Block Tick Not Inserted.");

            var utxoFeeInDatabase = await dbContext.UxtoFee.Where(w => 
                w.BlockTickGuid == blockTickInDatabase.Guid
                && blockTick.UtxoFees != null
                && w.LowFeePerKb == blockTick.UtxoFees.LowFeePerKb
                && w.MediumFeePerKb == blockTick.UtxoFees.MediumFeePerKb
                && w.HighFeePerKb == blockTick.UtxoFees.HighFeePerKb
                ).FirstOrDefaultAsync(_token);

            Assert.True(utxoFeeInDatabase != null, "Uxto Fee was not Inserted for Block Tick.");

            var gasFeeInDatabase = await dbContext.GasFee.Where(
                w => w.BlockTickGuid == blockTickInDatabase.Guid
                && blockTick.GasFees != null
                && w.BaseFee == blockTick.GasFees.BaseFee
                && w.HighGasPrice == blockTick.GasFees.HighGasPrice
                && w.MediumGasPrice == blockTick.GasFees.MediumGasPrice
                && w.LowGasPrice == blockTick.GasFees.LowGasPrice
                && w.HighPriorityFee == blockTick.GasFees.HighPriorityFee
                && w.MediumPriorityFee == blockTick.GasFees.MediumPriorityFee
                && w.LowPriorityFee == blockTick.GasFees.LowPriorityFee
                ).FirstOrDefaultAsync(_token);

            Assert.True(gasFeeInDatabase != null, "Gas Fee was not Inserted for Block Tick.");

            var rollback = false;
            try
            {
                await handler.Handle(command, _token);
            }
            catch (TransactionRollbackIdempotency)
            {
                rollback = true;
            }

            Assert.True(rollback, "Transaction was not rolled back on duplicate block tick.  Idempotency failed.");

        }
    }
}
