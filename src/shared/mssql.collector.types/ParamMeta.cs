using System.Collections.Generic;

using System.Runtime.Serialization;

namespace mssql.collector.types
{
    [DataContract]
    public class ParamMeta
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string SqlType { get; set; }

        [DataMember(Order = 3)]
        public List<TvpParamMeta> TVP { get; set; }

        [DataMember(Order = 4)]
        public bool HasDefaultValue { get; set; }

        [DataMember(Order = 5)]
        public int Order { get; set; }
    }
}