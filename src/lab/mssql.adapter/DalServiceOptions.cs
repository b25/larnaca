using System.Runtime.Serialization;

namespace mssql.adapter
{
    [DataContract]
    public class DalServiceOptions
    {
        [DataMember(Order = 1)]
        public string ConnectionString { get; set; }
        [DataMember(Order = 2)]
        public int MetricsLogInterval { get; set; } = 15;
        [DataMember(Order = 2)]
        public int MetricsMinLogDuration { get; set; } = 50;
    }
}