namespace Blockpit.Mediator.Handlers
{
    using Commands.Blockpit.Mediator.Commands;
    using Configuration;
    using Data;
    using Data.Models;
    using Exceptions;
    using LinqToDB;
    using log4net;
    using MediatR;
    using Models.Interfaces;
    using Observability;
    using Repository.Transactions;

    public class DatabaseStoreBlockHandler(ILog log, Settings settings, CentralCounterService counterService) : IRequestHandler<ProcessBlockCommand, Unit>
    {
        public async Task<Unit> Handle(ProcessBlockCommand request, CancellationToken cancellationToken)
        {
            counterService.AddEvent("BlockTickHandler");

            log.Info($"ProcessBlockCommand request received Name:{request.BlockTick.Name}; Hash: {request.BlockTick.Hash};");

            var handleGuid = Guid.NewGuid();
            var createdAt = DateTime.Now;

            log.Info($"ProcessBlockCommandGuid:{handleGuid}; assigned to Name:{request.BlockTick.Name}; Hash: {request.BlockTick.Hash};");

            var transactionOptions = new DataOptions().UseSQLite(settings.ConnectionString);
            using var blockTickTransaction = new BlockTickTransaction(new DbContext(transactionOptions));

            log.Info($"ProcessBlockCommandGuid:{handleGuid}; Created the distributed transaction wrapper (Unit of Work).");

            try
            {
                await blockTickTransaction.BeginTransactionAsync(cancellationToken);

                log.Info($"ProcessBlockCommandGuid:{handleGuid}; Starting transaction.");

                if (request.BlockTick is IBlockTick sourceBlockTick)
                {
                    var targetBlockTick = new BlockTick
                    {
                        Guid = handleGuid,
                        Symbol = sourceBlockTick.Symbol,
                        Name = sourceBlockTick.Name,
                        Height = sourceBlockTick.Height,
                        Hash = sourceBlockTick.Hash,
                        Time = sourceBlockTick.Time,
                        LatestUrl = sourceBlockTick.LatestUrl,
                        PreviousHash = sourceBlockTick.PreviousHash,
                        PreviousUrl = sourceBlockTick.PreviousUrl,
                        PeerCount = sourceBlockTick.PeerCount,
                        UnconfirmedCount = sourceBlockTick.UnconfirmedCount,
                        LastForkHeight = sourceBlockTick.LastForkHeight,
                        LastForkHash = sourceBlockTick.LastForkHash,
                        CreatedAt = createdAt
                    };

                    if (await blockTickTransaction.BlockTickRepository.GetByCompositeKeyAsync(targetBlockTick) != null)
                    {
                        counterService.AddEvent("BlockTickHandlerRollbackIdempotency");

                        log.Info($"ProcessBlockCommandGuid:{handleGuid}; Raising Rollback Idempotency.");

                        throw new TransactionRollbackIdempotency();
                    }

                    log.Info($"ProcessBlockCommandGuid:{handleGuid}; Inserting BlockTick.");

                    await blockTickTransaction.BlockTickRepository.AddAsync(targetBlockTick);

                    log.Info($"ProcessBlockCommandGuid:{handleGuid}; Inserted BlockTick.");

                    if (request.BlockTick.UtxoFees != null)
                    {
                        var targetUtxoFee = new UtxoFee
                        {
                            BlockTickGuid = handleGuid,
                            HighFeePerKb = request.BlockTick.UtxoFees.HighFeePerKb,
                            LowFeePerKb = request.BlockTick.UtxoFees.LowFeePerKb,
                            MediumFeePerKb = request.BlockTick.UtxoFees.MediumFeePerKb,
                            CreatedAt = DateTime.Now
                        };

                        log.Info($"ProcessBlockCommandGuid:{handleGuid}; Inserting UtxoFee.");

                        await blockTickTransaction.UtxoFeeRepository.AddAsync(targetUtxoFee);

                        log.Info($"ProcessBlockCommandGuid:{handleGuid}; Inserted UtxoFee.");
                    }

                    if (request.BlockTick.GasFees != null)
                    {
                        var targetGasFee = new GasFee
                        {
                            BlockTickGuid = handleGuid,
                            HighGasPrice = request.BlockTick.GasFees.HighGasPrice,
                            MediumGasPrice = request.BlockTick.GasFees.MediumGasPrice,
                            LowGasPrice = request.BlockTick.GasFees.LowGasPrice,
                            HighPriorityFee = request.BlockTick.GasFees.HighPriorityFee,
                            MediumPriorityFee = request.BlockTick.GasFees.MediumPriorityFee,
                            LowPriorityFee = request.BlockTick.GasFees.LowPriorityFee,
                            BaseFee = request.BlockTick.GasFees.BaseFee,
                            CreatedAt = DateTime.Now
                        };

                        log.Info($"ProcessBlockCommandGuid:{handleGuid}; Inserting GasFee.");

                        await blockTickTransaction.GasFeeRepository.AddAsync(targetGasFee);

                        log.Info($"ProcessBlockCommandGuid:{handleGuid}; Inserted GasFee.");
                    }

                    log.Info($"ProcessBlockCommandGuid:{handleGuid}; Committing.");

                    var committed = await blockTickTransaction.CommitAsync(handleGuid, cancellationToken);

                    if (committed)
                    {
                        counterService.AddEvent("BlockTickHandlerCommit");

                        log.Info($"ProcessBlockCommandGuid:{handleGuid}; Committed.");
                    }
                    else
                    {
                        counterService.AddEvent("BlockTickHandlerRollbackGraceful");

                        log.Info($"ProcessBlockCommandGuid:{handleGuid}; Raising Rollback Graceful.");

                        throw new TransactionRollbackGraceful();
                    }
                }
            }
            catch (Exception e) when (e is not TransactionRollbackIdempotency)
            {
                counterService.AddEvent("BlockTickHandlerRollbackFatal");

                log.Info($"ProcessBlockCommandGuid:{handleGuid}", e);

                await blockTickTransaction.RollbackAsync(handleGuid, cancellationToken);

                log.Info($"ProcessBlockCommandGuid:{handleGuid}; Raising Rollback Fatal.");

                throw new TransactionRollbackFatal();
            }

            log.Info($"ProcessBlockCommandGuid:{handleGuid}; Returning.");

            return Unit.Value;
        }
    }
}
