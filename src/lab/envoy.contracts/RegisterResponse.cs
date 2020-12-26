using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace envoy.contracts
{
    [DataContract]
    public class RegisterResponse
    {
        [DataMember(Order = 1)]
        public bool Registered { get; set; }

    }
}
