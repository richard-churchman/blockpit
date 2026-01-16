namespace Blockpit.Observability
{
    using System.Collections.Concurrent;
    using Microsoft.Extensions.Hosting;

    public class CentralCounterService : IHostedService, IDisposable
    {
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromSeconds(30);
        private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _counters = new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();
        private readonly TimeSpan _expiry = TimeSpan.FromMinutes(5);
        private Timer? _cleanupTimer;

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cleanupTimer = new Timer(Cleanup, null, TimeSpan.Zero, _cleanupInterval);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cleanupTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void AddEvent(string key)
        {
            var queue = _counters.GetOrAdd(key, _ => new ConcurrentQueue<DateTime>());
            queue.Enqueue(DateTime.Now);
        }

        public int GetEventCount(string key)
        {
            return _counters.TryGetValue(key, out var queue) ? queue.Count : 0;
        }

        public Dictionary<string, int> GetEventCounts()
        {
            return _counters.ToDictionary(counter => counter.Key, counter => counter.Value.Count);
        }

        private void Cleanup(object? state)
        {
            var now = DateTime.UtcNow;
            foreach (var (key, queue) in _counters)
            {

                while (queue.TryPeek(out var ts) && now - ts > _expiry)
                {
                    queue.TryDequeue(out _);
                }

                if (queue.IsEmpty)
                {
                    _counters.TryRemove(key, out _);
                }
            }
        }
    }
}
