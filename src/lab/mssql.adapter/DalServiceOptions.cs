using System.Runtime.Serialization;

namespace mssql.adapter
{
    [DataContract]
    public class DalServiceOptions
    {
        [DataMember(Order = 1)]
        public string ConnectionString { get; set; }

        [DataMember(Order = 2)]
        public string ConnectionUser { get; set; }

        [DataMember(Order = 3)]
        public string ConnectionPassword { get; set; }

        [DataMember(Order = 4)]
        public int MetricsLogInterval { get; set; } = 15;

        [DataMember(Order = 5)]
        public int MetricsLogDurationThreshold { get; set; } = 50;
    }
}