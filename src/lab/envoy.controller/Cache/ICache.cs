using Envoy.Service.Discovery.V3;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace envoy.controller.Cache
{
    public interface ICache
    {
        /// <summary>
        /// Returned task resolves when cache has values newer (or more resources) that passed in request
        /// </summary>
        Watch CreateWatch(DiscoveryRequest request);

        /// <summary>
        /// Returns current values from cache
        /// </summary>
        ValueTask<DiscoveryResponse> Fetch(DiscoveryRequest request);
    }

}
