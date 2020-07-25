using System.Collections.Generic;

using System.Runtime.Serialization;

namespace mssql.collector.types
{
    [DataContract]
    public class ProcedureMeta
    {
        [DataMember(Order = 1)]
        public string SpName { get; set; }

        [DataMember(Order = 2)]
        public List<ParamMeta> Request { get; set; }

        [DataMember(Order = 3)]
        public List<ResponseItem> Responses { get; set; } = new List<ResponseItem>();

        [DataMember(Order = 5)]
        public List<string> Errors { get; set; } = new List<string>();
    }
}