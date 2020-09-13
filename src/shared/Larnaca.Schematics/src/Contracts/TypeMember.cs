using System;
using System.Runtime.Serialization;


namespace Larnaca.Schematics
{
    [DataContract]
    public class TypeMember : ICloneable
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }
        [DataMember(Order = 2)]
        public TypeRef Type { get; set; }
        [DataMember(Order = 3)]
        public decimal? EnumValue { get; set; }
        [DataMember(Order = 4)]
        public MemberSerializationDetails SerializationDetails { get; set; }

        public TypeMember Clone() => new TypeMember()
        {
            Name = Name,
            Type = Type?.Clone(),
            EnumValue = EnumValue,
            SerializationDetails = SerializationDetails?.Clone(),
        };
        object ICloneable.Clone() => Clone();
    }
}
