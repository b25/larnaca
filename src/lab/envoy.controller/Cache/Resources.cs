using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace envoy.controller.Cache
{
    public class Resources
    {
        public string Version { get; }
        public IDictionary<string, IMessage> Items { get; }

        public Resources(string version, IDictionary<string, IMessage> items)
        {
            Items = items;
            Version = version;
        }
    }
}
