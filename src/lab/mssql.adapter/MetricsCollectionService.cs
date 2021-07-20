using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using mssql.adapter.Metrics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mssql.adapter
{
    public class MetricsCollectionService : EventListener, IHostedService
    {
        private ConcurrentBag<EventCounterData> _eventsData = new ConcurrentBag<EventCounterData>();
        private long _lastEventTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        private int _eventInterval;
        private int _eventMinLogDuration;

        public MetricsCollectionService(IOptions<DalServiceOptions> options)
        {
            _eventInterval = options.Value.MetricsLogInterval;
            _eventMinLogDuration = options.Value.MetricsMinLogDuration;
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (!eventSource.Name.Equals("mssql.utils.DataReader"))
            {
                return;
            }

            EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All, new Dictionary<string, string>
            {
                {"EventCounterIntervalSec", _eventInterval.ToString()}
            });
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= _lastEventTime + _eventInterval)
            {
                _lastEventTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var eventsData = Interlocked.Exchange(ref _eventsData, new ConcurrentBag<EventCounterData>());

                PrintToConsole(eventsData);
            }

            var counterData = eventData.ToEventCounterData();

            if (counterData == null)
            {
                Console.WriteLine($"Event received: {eventData.EventId} - {eventData.EventName}");
            }
            else if (counterData?.Count == 0)
            {
                return;
            }
            else
            {
                _eventsData.Add(counterData);
            }
        }

        private void PrintToConsole(ConcurrentBag<EventCounterData> eventsData)
        {
            var output = new StringBuilder();
            var slowCalls = eventsData.Where(x => x.Mean >= _eventMinLogDuration).OrderByDescending(x => x.Mean);

            foreach (var eventData in slowCalls)
            {
                output.AppendLine($"{eventData.Name.PadRight(70)}{eventData.Mean.ToString("0").PadRight(10)}{eventData.Min.ToString("0").PadRight(10)}{eventData.Max.ToString("0").PadRight(10)}{eventData.Count.ToString().PadRight(10)}");
            }

            if (output.Length > 0)
            {
                Console.WriteLine($"\x1b[33mTop slow calls in the last {_eventInterval} seconds:\n\n{"Stored Procedure".PadRight(70)}{"Mean".PadRight(10)}{"Min".PadRight(10)}{"Max".PadRight(10)}{"Count".PadRight(10)}\n{output}\x1b[39m");
            }
        }
    }
}
