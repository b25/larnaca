using ProtoBuf;
using System;
using System.Runtime.Serialization;

namespace Larnaca.Schematics
{
    [DataContract]
    public class ProtoContractDetails : ICloneable
    {
        public ProtoContractDetails() { }
        public ProtoContractDetails(Model model, Describer describer, ProtoContractAttribute attr)
        {
            FillFromAttr(model, describer, attr);
        }

        [DataMember(Order = 1)]
        public string Name { get; set; }
        [DataMember(Order = 2)]
        public int ImplicitFirstTag { get; set; }
        [DataMember(Order = 3)]
        public bool UseProtoMembersOnly { get; set; }
        [DataMember(Order = 4)]
        public bool IgnoreListHandling { get; set; }
        [DataMember(Order = 5)]
        public ImplicitFields ImplicitFields { get; set; }
        [DataMember(Order = 6)]
        public bool InferTagFromName { get; set; }
        [DataMember(Order = 7)]
        public int DataMemberOffset { get; set; }
        [DataMember(Order = 8)]
        public bool SkipConstructor { get; set; }
        [DataMember(Order = 9)]
        public bool AsReferenceDefault { get; set; }
        [DataMember(Order = 10)]
        public bool IsGroup { get; set; }
        [DataMember(Order = 11)]
        public bool EnumPassthru { get; set; }
        [DataMember(Order = 12)]
        public TypeRef Surrogate { get; set; }

        public void FillFromAttr(Model model, Describer describer, ProtoContractAttribute attr)
        {
            Name = attr.Name;
            ImplicitFirstTag = attr.ImplicitFirstTag;
            UseProtoMembersOnly = attr.UseProtoMembersOnly;
            IgnoreListHandling = attr.IgnoreListHandling;
            ImplicitFields = attr.ImplicitFields;
            InferTagFromName = attr.InferTagFromName;
            DataMemberOffset = attr.DataMemberOffset;
            SkipConstructor = attr.SkipConstructor;
            AsReferenceDefault = attr.AsReferenceDefault;
            IsGroup = attr.IsGroup;
            //EnumPassthru = attr.EnumPassthru;
            if (attr.Surrogate != null)
            {
                Surrogate = describer.GetNameAddWithReferencesToModel(model, attr.Surrogate);
            }
        }

        public ProtoContractDetails Clone() => new ProtoContractDetails()
        {
            Name = Name,
            ImplicitFirstTag = ImplicitFirstTag,
            UseProtoMembersOnly = UseProtoMembersOnly,
            IgnoreListHandling = IgnoreListHandling,
            ImplicitFields = ImplicitFields,
            InferTagFromName = InferTagFromName,
            SkipConstructor = SkipConstructor,
            AsReferenceDefault = AsReferenceDefault,
            IsGroup = IsGroup,
            EnumPassthru = EnumPassthru,
            Surrogate = Surrogate?.Clone()
        };
        object ICloneable.Clone() => Clone();
    }
}
