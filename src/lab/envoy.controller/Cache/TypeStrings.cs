using System;
using System.Collections.Generic;
using System.Text;

namespace envoy.controller.Cache
{

    public static class TypeStrings
    {
        public const string TypePrefix = "type.googleapis.com/envoy.config.";
        public const string EndpointType = TypePrefix + "endpoint.v3.ClusterLoadAssignment";
        public const string ClusterType = TypePrefix + "cluster.v3.Cluster";
        public const string RouteType = TypePrefix + "route.v3.RouteConfiguration";
        public const string ListenerType = TypePrefix + "listener.v3.Listener";
        public const string Any = "";

        public static int GetPriority(string type)
        {
            switch (type)
            {
                case TypeStrings.ClusterType:
                    return 0;
                case TypeStrings.EndpointType:
                    return 1;
                case TypeStrings.ListenerType:
                    return 2;
                case TypeStrings.RouteType:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
