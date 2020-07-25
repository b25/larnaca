using System.Runtime.Serialization;

namespace mssql.collector.types
{
    [DataContract]
    public class TvpParamMeta
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string SqlType { get; set; }

        [DataMember(Order = 3)]
        public bool IsNullable { get; set; }

        [DataMember(Order = 4)]
        public int Order { get; set; }
    }
}