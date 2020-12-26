using System.Runtime.Serialization;

namespace envoy.contracts
{
    [DataContract]
    public class UnregisterResponse
    {
        [DataMember(Order = 1)]
        public bool Unregistered { get; set; }

    }
}
