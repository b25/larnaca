using System;
using System.Runtime.Serialization;


namespace Larnaca.Schematics
{
    [DataContract]
    public class DataMemberDetails : ICloneable
    {
        /// <summary>
        /// emit default value is always true, unless explicitly set to false
        /// </summary>
        [DataMember(Order = 1)]
        public bool? EmitDefaultValue { get; set; }

        [DataMember(Order = 2)]
        public bool? IsRequired { get; set; }

        [DataMember(Order = 3)]
        public string Name { get; set; }

        [DataMember(Order = 4)]
        public int? Order { get; set; }

        [DataMember(Order = 5)]
        public bool? Ignore { get; set; }

        public void FillFromAttribute(DataMemberAttribute dma)
        {
            // emit default value is always true, unless explicitly set to false
            if (!dma.EmitDefaultValue)
            {
                EmitDefaultValue = dma.EmitDefaultValue;
            }
            if (dma.IsRequired)
            {
                IsRequired = dma.IsRequired;
            }
            if (dma.IsNameSetExplicitly)
            {
                Name = dma.Name;
            }
            if (dma.Order == -1)
            {
                Order = null;
            }
            else
            {
                Order = dma.Order;
            }
        }

        public DataMemberDetails Clone() => new DataMemberDetails()
        {
            EmitDefaultValue = EmitDefaultValue,
            IsRequired = IsRequired,
            Name = Name,
            Order = Order,
            Ignore = Ignore
        };
        object ICloneable.Clone() => Clone();
    }
}
