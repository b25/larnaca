﻿using System.Runtime.Serialization;

namespace envoy.controller
{
    [DataContract]
    public class GatewayOptions
    {
        [DataMember(Order = 1)]
        public uint ListenPort { get; set; } = 3000;

        [DataMember(Order = 1)]
        public uint PerConnectionBufferLimitBytes { get; set; } = 32768; // 32 KiB

        [DataMember(Order = 1)]
        public uint PeerMaxConcurrentStreams { get; set; } = 100;

        [DataMember(Order = 1)]
        public uint InitialStreamWindowSize { get; set; } = 65536; // 64 KiB

        [DataMember(Order = 1)]
        public uint InitialConnectionWindowSize { get; set; } = 1048576; // 1 MiB

        [DataMember(Order = 1)]
        public uint RouteTimeoutSeconds { get; set; } = 0;
    }
}