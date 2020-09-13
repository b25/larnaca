using System;
using System.Runtime.Serialization;

namespace Larnaca.Schematics
{
    [DataContract]
    public class KnownTypeItem : ICloneable
    {
        public KnownTypeItem() { }

        public KnownTypeItem(Model model, Describer describer, KnownTypeAttribute attr)
        {
            FillFromAttribute(model, describer, attr);
        }

        [DataMember(Order = 1)]
        public string MethodName { get; set; }
        [DataMember(Order = 2)]
        public TypeRef Type { get; set; }

        public void FillFromAttribute(Model model, Describer describer, KnownTypeAttribute attr)
        {
            MethodName = attr.MethodName;
            Type = describer.GetTypeRefAddOutlineToModel(model, attr.Type);
        }

        public KnownTypeItem Clone() => new KnownTypeItem()
        {
            MethodName = MethodName,
            Type = Type.Clone(),
        };
        object ICloneable.Clone() => Clone();
    }
}