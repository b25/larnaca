using Grpc.AspNetCore.Server;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace envoy.contracts
{
    [DataContract]
    public class RegisterRequest
    {
        [DataMember(Order = 1)]
        public string NodeName { get; set; }

        [DataMember(Order = 2)]
        public string NodeIp { get; set; }

        [DataMember(Order = 3)]
        public string Namespace { get; set; }

        [DataMember(Order = 4)]
        public string PodName { get; set; }

        [DataMember(Order = 5)]
        public string PodAddress { get; set; }

        [DataMember(Order = 6)]
        public uint PodPort { get; set; }

        [DataMember(Order = 7)]
        public List<string> Routes { get; set; } = new List<string>();

        [DataMember(Order = 8)]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Retrieves the routes for the given service types.
        /// </summary>
        /// <param name="endpointDataSource"></param>
        /// <param name="serviceTypes"></param>
        /// <returns></returns>
        public static List<string> GetRoutes(EndpointDataSource endpointDataSource, IList<Type> serviceTypes)
        {
            var grpcEndpointMetadata = endpointDataSource.Endpoints
                .Select(endpoint => endpoint.Metadata.GetMetadata<GrpcMethodMetadata>())
                .Where(metadata => metadata != null && (serviceTypes.Contains(metadata.ServiceType) || serviceTypes.Any(type => type.IsAssignableFrom(metadata.ServiceType))))
                .ToList();

            return grpcEndpointMetadata
                .Select(metadata => metadata.Method.FullName)
                .ToList();
        }
    }
}
