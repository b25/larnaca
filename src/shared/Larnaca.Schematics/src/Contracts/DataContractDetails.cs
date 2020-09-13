using System;
using System.Runtime.Serialization;

namespace Larnaca.Schematics
{
    [DataContract]
    public class DataContractDetails : ICloneable
    {
        public DataContractDetails() { }
        public DataContractDetails(DataContractAttribute attr)
        {
            FillFromAttr(attr);
        }

        [DataMember(Order = 1)]
        public bool? IsReference { get; set; }
        [DataMember(Order = 2)]
        public string Name { get; set; }
        [DataMember(Order = 3)]
        public string Namespace { get; set; }
        public void FillFromAttr(DataContractAttribute attr)
        {
            if (attr.IsReferenceSetExplicitly)
            {
                IsReference = attr.IsReference;
            }

            if (attr.IsNameSetExplicitly)
            {
                Name = attr.Name;
            }

            if (attr.IsNamespaceSetExplicitly)
            {
                Namespace = attr.Namespace;
            }
        }
        public DataContractDetails Clone() => new DataContractDetails()
        {
            IsReference = IsReference,
            Name = Name,
            Namespace = Namespace
        };
        object ICloneable.Clone() => Clone();
    }
}