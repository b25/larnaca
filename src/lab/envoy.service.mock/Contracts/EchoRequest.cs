using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace envoy.service.Contracts
{
    [DataContract]
    public class EchoRequest
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }

    }
}
