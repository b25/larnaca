using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace mssql.adapter.Metrics
{
    /// <summary>
    /// Extensions methods for <see cref="EventWrittenEventArgs"/>
    /// </summary>
    public static class EventWrittenEventArgsExtensions
    {
        public static bool IsEventCounter(this EventWrittenEventArgs eventData)
        {
            return eventData.EventName == "EventCounters";
        }

        public static EventCounterData ToEventCounterData(this EventWrittenEventArgs eventData)
        {
            if (!eventData.IsEventCounter() || eventData.Payload.Count <= 0)
                return null;

            return new EventCounterData(eventData);
        }
    }
}
