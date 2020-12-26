using System.Runtime.Serialization;

namespace envoy.contracts
{
    [DataContract]
    public class UnregisterRequest
    {
        [DataMember(Order = 1)]
        public string PodAddress { get; set; }

        [DataMember(Order = 2)]
        public uint PodPort { get; set; }
    }
}
