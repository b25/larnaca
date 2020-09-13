using Larnaca.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Larnaca.Schematics
{
    [DataContract]
    public class TypeOutline : ICloneable
    {
        [DataMember(Order = 1)]
        public TypeRef Name { get; set; }
        [DataMember(Order = 2)]
        public TypeClassification Classification { get; set; }
        [DataMember(Order = 3)]
        public TypeMember[] Members { get; set; }
        [DataMember(Order = 4)]
        public TypeRef[] ImplementedInterfaces { get; set; }
        [DataMember(Order = 6)]
        public TypeRef Base { get; set; }
        [DataMember(Order = 7)]
        public TypeSerializationDetails SerializationDetails { get; set; }
        [DataMember(Order = 8)]
        public Dictionary<TypeRef /*generic type parameter */, string[] /* member name */> GenericMembers { get; set; }
        [DataMember(Order = 9)]
        public string Assembly { get; set; }

        public TypeMember[] GetNonInheritedMembers(Model model)
        {
            if (Base == null || Name.IsEnum)
            {
                return Members;
            }
            else
            {
                var baseOutline = model.Outlines[Base];
                return Members.EmptyIfNull().Where(m => !baseOutline.Members.Any(bm => bm.Name == m.Name)).ToArray();
            }
        }

        public TypeOutline Clone() => new TypeOutline()
        {
            Name = Name?.Clone(),
            Assembly = Assembly,
            Base = Base?.Clone(),
            Classification = Classification,
            GenericMembers = GenericMembers?.ToDictionary(kvp => kvp.Key.Clone(), kvp => kvp.Value),
            ImplementedInterfaces = ImplementedInterfaces?.Select(i => i.Clone()).ToArray(),
            Members = Members?.Select(m => m.Clone()).ToArray(),
            SerializationDetails = SerializationDetails?.Clone(),
        };


        public HashSet<TypeRef> SelfAndReferencedNames() => AddSelfAndReferencedTypeRefs(new HashSet<TypeRef>(new ObjectReferenceEqualityComparer<TypeRef>()));

        public HashSet<TypeRef> AddSelfAndReferencedTypeRefs(HashSet<TypeRef> existingList)
        {
            Name?.AddSelfAndReferencedTypeRefs(existingList);
            Base?.AddSelfAndReferencedTypeRefs(existingList);
            foreach (var currentInterface in ImplementedInterfaces.EmptyIfNull())
            {
                currentInterface?.AddSelfAndReferencedTypeRefs(existingList);
            }
            foreach (var currentMember in Members.EmptyIfNull())
            {
                if (currentMember != null)
                {
                    currentMember.Type?.AddSelfAndReferencedTypeRefs(existingList);
                }
            }
            foreach (var currentKnownType in SerializationDetails?.KnownTypeItems.EmptyIfNull())
            {
                if (currentKnownType != null)
                {
                    currentKnownType.Type?.AddSelfAndReferencedTypeRefs(existingList);
                }
            }
            foreach (var currentProtoInclude in SerializationDetails?.ProtoIncludeItems.EmptyIfNull())
            {
                if (currentProtoInclude != null)
                {
                    currentProtoInclude.KnownType?.AddSelfAndReferencedTypeRefs(existingList);
                }
            }
            return existingList;
        }

        object ICloneable.Clone() => Clone();

        public OperationResult<TypeOutline> GenerateConstructedOutline(TypeRef constructedName)
        {
            if (constructedName is null)
            {
                return new OperationResult<TypeOutline>("constructedName is null");
            }

            if (!constructedName.IsBasicallySameType(Name))
            {
                return new OperationResult<TypeOutline>("provided constructedName is not the same name as this outline");
            }

            if (!(Name.IsGenericTypeDefinition && constructedName.IsConstructedGenericType))
            {
                return new OperationResult<TypeOutline>("the provided constructedName should be a ConstructedGenericType and this outline should be a GenericTypeDefinition");
            }

            var theReturn = Clone();
            Dictionary<TypeRef, TypeRef> substitutions = new Dictionary<TypeRef, TypeRef>();

            for (int i = 0; i < theReturn.Name.GenericParameters.Length; i++)
            {
                substitutions[Name.GenericParameters[i]] = constructedName.Wrapped[i];
            }

            foreach (var currentMember in theReturn.Members)
            {
                currentMember.Type = replaceGenericparamsWithActualTypes(currentMember.Type);
            }

            return theReturn.ToOperationResult();

            TypeRef replaceGenericparamsWithActualTypes(TypeRef name)
            {
                if (name.IsArray)
                {
                    var nonArrayReturn = replaceGenericparamsWithActualTypes(name.GetNonArrayType());
                    nonArrayReturn = nonArrayReturn.Clone();
                    nonArrayReturn.ArrayDimensions = name.ArrayDimensions;
                    return nonArrayReturn;
                }

                if (substitutions.TryGetValue(name, out var theReturn))
                {
                    return theReturn;
                }

                for (int i = 0; i < name.Wrapped?.Length; i++)
                {
                    name.Wrapped[i] = replaceGenericparamsWithActualTypes(name.Wrapped[i]);
                }

                return name;
            }
        }
    }
}
