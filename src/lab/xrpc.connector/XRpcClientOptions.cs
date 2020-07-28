using System.Runtime.Serialization;

namespace xrpc.connector
{
    [DataContract]
    public class XRpcClientOptions
    {
        [DataMember(Order= 1)]
        public XRPCChannelType ChannelType { get; set; } = XRPCChannelType.GRPC;
       
        [DataMember(Order=2)]
        public string Url { get; set; }

    }
}
