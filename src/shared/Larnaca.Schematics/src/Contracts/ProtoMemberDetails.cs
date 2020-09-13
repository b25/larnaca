using ProtoBuf;
using System;
using System.Runtime.Serialization;

namespace Larnaca.Schematics
{
    [DataContract]
    public class ProtoMemberDetails : ICloneable
    {
        [DataMember(Order = 1)]
        public int Tag { get; set; }
        [DataMember(Order = 2)]
        public MemberSerializationOptions Options { get; set; }
        [DataMember(Order = 3)]
        public bool? Ignore { get; set; }
        public void FillFromAttribute(ProtoMemberAttribute pma)
        {
            Tag = pma.Tag;
            Options = pma.Options;
        }
        public ProtoMemberDetails Clone() => new ProtoMemberDetails()
        {
            Tag = Tag,
            Options = Options,
            Ignore = Ignore,
        };

        object ICloneable.Clone() => Clone();
    }
}
