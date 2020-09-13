using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Larnaca.Schematics
{
    [DataContract]
    public class TypeSerializationDetails : ICloneable
    {
        [DataMember(Order = 1)]
        public DataContractDetails DataContractDetails { get; set; }
        [DataMember(Order = 2)]
        public ProtoContractDetails ProtoContractDetails { get; set; }
        [DataMember(Order = 3)]
        public KnownTypeItem[] KnownTypeItems { get; set; }
        [DataMember(Order = 4)]
        public ProtoIncludeItem[] ProtoIncludeItems { get; set; }
        public TypeSerializationDetails Clone() => new TypeSerializationDetails()
        {
            DataContractDetails = DataContractDetails?.Clone(),
            ProtoContractDetails = ProtoContractDetails?.Clone(),
            KnownTypeItems = KnownTypeItems?.Select(i => i.Clone()).ToArray(),
            ProtoIncludeItems = ProtoIncludeItems?.Select(i => i.Clone()).ToArray()
        };
        object ICloneable.Clone() => Clone();
    }
}