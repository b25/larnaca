using ProtoBuf;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Larnaca.Schematics
{
    [DataContract]
    public class ProtoIncludeItem : ICloneable
    {
        public ProtoIncludeItem() { }
        public ProtoIncludeItem(Model model, Describer describer, ProtoIncludeAttribute attr)
        {
            FillFromAttr(model, describer, attr);
        }
        [DataMember(Order = 1)]
        public int Tag { get; set; }
        [DataMember(Order = 2)]
        public string KnownTypeName { get; set; }
        [DataMember(Order = 3)]
        public TypeRef KnownType { get; set; }
        [DataMember(Order = 4)]
        public DataFormat DataFormat { get; set; }

        public void FillFromAttr(Model model, Describer describer, ProtoIncludeAttribute attr)
        {
            Tag = attr.Tag;
            KnownTypeName = attr.KnownTypeName;
            if (attr.KnownType != null)
            {
                KnownType = describer.GetTypeRefAddOutlineToModel(model, attr.KnownType);
            }
            else
            {
                var knownType = Type.GetType(attr.KnownTypeName, false);
                if (knownType != null)
                {
                    KnownType = describer.GetTypeRefAddOutlineToModel(model, knownType);
                }
            }
            DataFormat = attr.DataFormat;
        }

        public ProtoIncludeItem Clone() => new ProtoIncludeItem()
        {
            Tag = Tag,
            KnownTypeName = KnownTypeName,
            KnownType = KnownType?.Clone(),
            DataFormat = DataFormat
        };
        object ICloneable.Clone() => Clone();
    }
}