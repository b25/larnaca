using Envoy.Service.Discovery.V3;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace envoy.controller.Cache
{
    public struct Watch
    {
        internal static readonly Action NoOp = () => { };
        private readonly Action _cancel;

        public static readonly Watch Empty = new Watch(new TaskCompletionSource<DiscoveryResponse>().Task, NoOp);

        public Watch(Task<DiscoveryResponse> response, Action cancel)
        {
            Response = response;
            _cancel = cancel;
        }

        public Task<DiscoveryResponse> Response { get; }

        public void Cancel()
        {
            _cancel();
        }
    }
}
