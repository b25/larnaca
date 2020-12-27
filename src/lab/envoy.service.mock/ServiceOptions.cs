using System.Runtime.Serialization;

namespace envoy.service
{
    [DataContract]
    public class ServiceOptions
    {
        [DataMember(Order = 1)]
        public string GatewayAddress { get; set; } = "http://localhost:5000";
    }
}