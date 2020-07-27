using System;
using System.Collections.Concurrent;

namespace xrpc.connector
{

    public partial class XRpcClient
    {
    
        XRpcClientOptions Options;

        public XRpcClient(XRpcClientOptions options)
        {
            Options = options;
        }

    }
}
