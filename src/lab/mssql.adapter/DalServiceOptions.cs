using System.Runtime.Serialization;

namespace mssql.adapter
{
    [DataContract]
    public class DalServiceOptions
    {
        [DataMember(Order = 1)]
        public string ConnectionString { get; set; }
    }
}