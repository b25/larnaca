using System.Collections.Generic;
using System.Runtime.Serialization;

namespace mssql.collector.types
{
    [DataContract]
    public class ResponseItem
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public List<ParamMeta> Params { get; set; }

        [DataMember(Order = 3)]
        public int Order { get; set; }
    }
}