using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace mssql.utils
{
    [EventSource(Name = "mssql.utils")]
    public class MetricsEventSource : EventSource
    {
        public static MetricsEventSource Instance = new MetricsEventSource();
        private readonly ConcurrentDictionary<string, EventCounter> _dynamicCounters = new ConcurrentDictionary<string, EventCounter>();

        private MetricsEventSource() { }

        public void RecordMetric(string name, float value)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            var counter = _dynamicCounters.GetOrAdd(name, key => new EventCounter(key, this));
            counter.WriteMetric(value);
        }
    }
}
