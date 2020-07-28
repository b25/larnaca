using Grpc.Core;
using Grpc.Net.Client;
using mssql.adapter;
using ProtoBuf.Grpc.Client;
using System;
using System.Collections.Concurrent;

namespace xrpc.connector
{

    public partial class XRpcClient
    {
    
        XRpcClientOptions Options;
        GrpcChannel channel;
        public XRpcClient(XRpcClientOptions options)
        {
            if (!Uri.IsWellFormedUriString(options.Url,UriKind.Absolute))
            {
                throw new Exception($"XRpc invalid URI");
            }
            Options = options;
            channel = GrpcChannel.ForAddress(options.Url);

            var service = channel.CreateGrpcService<IDalService>();
            //todo
            }

    }
}
