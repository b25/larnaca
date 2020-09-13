using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Larnaca.Schematics
{
    [DataContract]
    public class Model : ICloneable
    {
        [DataMember(Order = 1)]
        public Dictionary<TypeRef, TypeOutline> Outlines { get; set; } = new Dictionary<TypeRef, TypeOutline>(new AproxTypeRefComparer());
        //[DataMember(Order = 2)]
        //public List<Service> Services { get; set; } = new List<Service>();

        public Model Clone()
        {
            var theReturn = new Model();
            foreach (var currentOutline in Outlines.Values)
            {
                var currentClone = currentOutline.Clone();
                theReturn.Outlines[currentClone.Name] = currentClone;
            }
            //foreach (var currentService in Services)
            //{
            //    theReturn.Services.Add(currentService.Clone());
            //}
            return theReturn;
        }
        object ICloneable.Clone() => Clone();
    }
}
