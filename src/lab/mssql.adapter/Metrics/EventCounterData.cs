using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace mssql.adapter.Metrics
{
    public class EventCounterData
    {
        public string Name { get; }
        public double Mean { get; }
        public double StandardDeviation { get; }
        public int Count { get; }
        public float IntervalSec { get; }
        public double Min { get; }
        public double Max { get; }

        public EventCounterData(EventWrittenEventArgs eventData)
        {
            var payload = (IDictionary<string, object>)eventData.Payload[0];

            Name = payload["Name"].ToString();
            Mean = (double)payload["Mean"];
            StandardDeviation = (double)payload["StandardDeviation"];
            Count = (int)payload["Count"];
            IntervalSec = (float)payload["IntervalSec"];
            Min = (double)payload["Min"];
            Max = (double)payload["Max"];
        }
    }
}
