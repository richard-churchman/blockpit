namespace Blockpit.Listener.PollToPublishTasks
{
    using System.Text.Json;
    using Helpers;
    using Interfaces;
    using log4net;
    using Mediator.Commands.Blockpit.Mediator.Commands;
    using Mediator.Exceptions;
    using Mediator.Models;
    using MediatR;
    using Models;
    using Observability;

    public class BlockCypherDashPollToPublishTask(
        ILog log,
        IMediator mediator,
        CentralCounterService counterService,
        string url,
        bool ignoreSsl = false,
        int pollRate = 10000,
        CancellationToken token = default) : IPollToPublishTask
    {
        public async Task StartAsync()
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            };

            var hash = String.Empty;
            var height = 0;
            var lastForHash = String.Empty;

            log.Info($"Starting Url: {url};");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    log.Info($"Fetching from Url: {url};");

                    var response = await HttpJsonHelper.GetAsync<BlockCypherDash>(url, jsonSerializerOptions, ignoreSsl);

                    log.Info($"Url: {url}; Response: {response}");

                    if (String.IsNullOrEmpty(response.Hash) || response.Height == null || String.IsNullOrEmpty(response.LastForkHash))
                    {
                        log.Info($"Url: {url}; has bad data;");

                        continue;
                    }

                    if (response.Hash == hash && height == response.Height && response.LastForkHash == lastForHash)
                    {
                        counterService.AddEvent("DASHPollRepeat");

                        log.Info($"Url: {url}; is a repeat;");

                        continue;
                    }

                    hash = response.Hash;
                    height = response.Height.Value;
                    lastForHash = response.LastForkHash;

                    var block = new BlockTick(
                        "DASH",
                        response.Name ?? String.Empty,
                        response.Height ?? 0,
                        response.Hash ?? String.Empty,
                        response.Time ?? DateTime.Now,
                        response.LatestUrl ?? String.Empty,
                        response.PreviousHash ?? String.Empty,
                        response.PreviousUrl ?? String.Empty,
                        response.PeerCount ?? 0,
                        response.UnconfirmedCount ?? 0,
                        response.LastForkHeight ?? 0,
                        response.LastForkHash ?? String.Empty
                    );

                    if (response is { HighFeePerKb: not null, MediumFeePerKb: not null, LowFeePerKb: not null })
                    {
                        log.Info($"Url: {url}; has UtxFees");

                        block.SetUtxoFees(
                            response.HighFeePerKb.Value,
                            response.MediumFeePerKb.Value,
                            response.LowFeePerKb.Value
                        );
                    }

                    log.Info($"Url: {url}; sending to Mediator.");

                    await mediator.Send(new ProcessBlockCommand(block));

                    counterService.AddEvent("DASHPoll");

                    log.Info($"Url: {url}; Mediator finished.");
                }
                catch (TransactionRollbackIdempotency)
                {
                    counterService.AddEvent("DASHPollRepeatMediator");

                    log.Info($"Url: {url}; is a repeat;");
                }
                catch (Exception e)
                {
                    counterService.AddEvent("DASHPollErrors");

                    log.Error($"Url: {url}", e);
                }
                finally
                {
                    log.Info($"Url: {url}; waiting.");

                    await Task.Delay(pollRate, token);

                    log.Info($"Url: {url}; ready.");
                }

                log.Info($"Stopped Url: {url};");
            }
        }
    }
}
