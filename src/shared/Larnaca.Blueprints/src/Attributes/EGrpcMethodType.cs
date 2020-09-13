namespace Larnaca.Blueprints
{
    /// <summary>
    /// Compatible with Grpc.Core.MethodType
    /// </summary>
    public enum EGrpcMethodType
    {
        /// <summary>
        /// Single request sent from client, single response received from server.
        /// </summary>
        Unary,
        /// <summary>
        /// Stream of request sent from client, single response received from server.
        /// </summary>
        ClientStreaming,
        /// <summary>
        /// Single request sent from client, stream of responses received from server.
        /// </summary>
        ServerStreaming,
        /// <summary>
        /// Both server and client can stream arbitrary number of requests and responses simultaneously.
        /// </summary>
        DuplexStreaming
    }
}