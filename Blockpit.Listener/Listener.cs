namespace Blockpit.Listener
{
    using Configuration;
    using log4net;
    using MediatR;
    using Microsoft.Extensions.Hosting;
    using Observability;
    using PollToPublishTasks;

    public class Listener(ILog log, Settings settings, IMediator mediator, CentralCounterService counterService) : IHostedService

    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly List<Task> _tasks = [];

        public Task StartAsync(CancellationToken token)
        {
            log.Info("Starting the Listener Hosted Service.");

            var blockCypherBtcTask = new BlockCypherBtcPollToPublishTask(log, mediator, counterService, settings.BlockCypherBtcUrl, settings.IgnoreSsl, settings.PollRate, _cancellationTokenSource.Token);
            _tasks.Add(blockCypherBtcTask.StartAsync());

            log.Info($"Started BlockCypherBtcPollToPublishTask on Url: {settings.BlockCypherBtcUrl};");

            var blockCypherDashTask = new BlockCypherDashPollToPublishTask(log, mediator, counterService, settings.BlockCypherDashUrl, settings.IgnoreSsl, settings.PollRate, _cancellationTokenSource.Token);
            _tasks.Add(blockCypherDashTask.StartAsync());

            log.Info($"Started BlockCypherDashPollToPublishTask on Url: {settings.BlockCypherDashUrl};");

            var blockCypherEthTask = new BlockCypherEthPollToPublishTask(log, mediator, counterService, settings.BlockCypherEthUrl, settings.IgnoreSsl, settings.PollRate, _cancellationTokenSource.Token);
            _tasks.Add(blockCypherEthTask.StartAsync());

            log.Info($"Started BlockCypherEthPollToPublishTask on Url: {settings.BlockCypherEthUrl};");

            log.Info($"Started {_tasks.Count} tasks. Returning.");

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken token)
        {
            await _cancellationTokenSource.CancelAsync();

            log.Info("Cancellation called in listener.");

            log.Info($"About checked stopped {_tasks.Count}; tasks.");

            while (_tasks.Any(t => !t.IsCompleted))
            {
                log.Info($"Waiting on teardown for {_tasks.Count}; tasks.");

                try
                {
                    await Task.Delay(1000, token);
                }
                catch (OperationCanceledException)
                {
                    log.Warn("StopAsync canceled by host shutdown.  Not otherwise implemented.");

                    break;
                }
            }

            log.Info("Listener stopped.");
        }
    }
}
