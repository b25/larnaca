using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Larnaca.Schematics
{
    [DataContract]
    public class MemberSerializationDetails : ICloneable
    {
        [DataMember(Order = 1)]
        public DataMemberDetails DataMemberDetails { get; set; }
        [DataMember(Order = 2)]
        public ProtoMemberDetails ProtoMemberDetails { get; set; }
        [DataMember(Order = 3)]
        public bool SensitiveInfo { get; set; }
        public MemberSerializationDetails Clone() => new MemberSerializationDetails()
        {
            DataMemberDetails = DataMemberDetails?.Clone(),
            ProtoMemberDetails = ProtoMemberDetails?.Clone(),
            SensitiveInfo = SensitiveInfo
        };
        object ICloneable.Clone() => Clone();
    }
}
