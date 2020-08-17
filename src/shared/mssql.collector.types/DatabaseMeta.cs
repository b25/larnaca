using System;
using System.Runtime.Serialization;

namespace mssql.collector.types
{
    [Serializable]
    [DataContract]
    public class DatabaseMeta
    {
        [DataMember(Order = 1)]
        public string Name;

        [DataMember(Order = 2)]
        public ProcedureMeta[] Procedures;
    }
}