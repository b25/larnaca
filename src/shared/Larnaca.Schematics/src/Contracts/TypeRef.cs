using Larnaca.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Larnaca.Schematics
{
    [DataContract]
    public class TypeRef : ICloneable, IEquatable<TypeRef>
    {

        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string? Namespace { get; set; }

        [DataMember(Order = 3)]
        public TypeRef? NestedIn { get; set; }

        /// <summary>
        /// For a constructed generic type contains the types of the generic arguments        
        /// e.g. Dictionary<int,string> will contain [int] and [string]        
        /// </summary>
        [DataMember(Order = 4)]
        public TypeRef[]? Wrapped { get; set; }

        /// <summary>
        /// For a generic type definition contains the names of the generic arguments, otherwise null
        /// e.g. Dictionary[,] will contain: [2]{ "TKey", "TValue" }
        /// </summary>
        [DataMember(Order = 5)]
        public TypeRef[]? GenericParameters { get; set; }

        [DataMember(Order = 6)]
        public int ArrayDimensions { get; set; }
        /// <summary>
        /// A generic type definition is a template from which other types can be constructed.
        /// e.g. Dictionary[,]
        /// </summary>
        [DataMember(Order = 7)]
        public bool IsGenericTypeDefinition { get; set; }

        /// <summary>
        /// A constructed generic type has had explicit types supplied for all of its generic type parameters. It is also referred to as a closed generic type.
        /// e.g. Dictionary[int,string]
        /// </summary>
        [DataMember(Order = 8)]
        public bool IsConstructedGenericType { get; set; }

        /// <summary>
        /// A generic type parameter is a type parameter in the definition of a generic type or method.
        /// e.g. TKey and TValue in Dictionary[TKey,TValue]
        /// </summary>
        [DataMember(Order = 9)]
        public bool IsGenericParameter { get; set; }
        [DataMember(Order = 10)]
        public bool IsPrimitive { get; set; }
        [DataMember(Order = 11)]
        public bool IsWellKnown { get; set; }
        [DataMember(Order = 12)]
        public bool IsEnum { get; set; }
        [DataMember(Order = 13)]
        public bool IsInterface { get; set; }
        [IgnoreDataMember]
        public bool IsArray => ArrayDimensions > 0;
        [IgnoreDataMember]
        public bool IsGeneric => IsConstructedGenericType || IsGenericTypeDefinition || IsGenericParameter;

        public bool IsBasicallySameType(TypeRef other) => AproxTypeRefComparer.Equals(this, other);
        public string GenerateNameWithoutGenericsAndArrays()
        {
            string theReturn;
            if (NestedIn != null)
            {
                theReturn = $"{NestedIn.GenerateName()}.{Name}";
            }
            else if (!string.IsNullOrEmpty(Namespace))
            {
                theReturn = $"{Namespace}.{Name}";
            }
            else
            {
                theReturn = Name;
            }

            if (theReturn == null)
            {
                theReturn = string.Empty;
            }
            return theReturn;
        }
        public string GenerateReflectionNameWithoutGenericsAndArrays()
        {
            string theReturn;
            if (NestedIn != null)
            {
                theReturn = $"{NestedIn.GenerateReflectionName()}+{Name}";
            }
            else if (!string.IsNullOrEmpty(Namespace))
            {
                theReturn = $"{Namespace}.{Name}";
            }
            else
            {
                theReturn = Name;
            }

            if (theReturn == null)
            {
                theReturn = string.Empty;
            }
            return theReturn;
        }
        /// <summary>
        /// An easily readable name
        /// </summary>
        public string GenerateName(bool prefixWithGlobal = false)
        {
            if (IsGenericParameter)
            {
                return Name;
            }
            string theReturn = GenerateNameWithoutGenericsAndArrays();
            if (IsGenericTypeDefinition)
            {
                theReturn = $"{theReturn}<{string.Join(",", GenericParameters.Select(t => t.Name))}>";
            }
            else if (IsConstructedGenericType)
            {
                theReturn = $"{theReturn}<{string.Join(",", Wrapped.Select(t => t.GenerateName(prefixWithGlobal)))}>";
            }

            if (ArrayDimensions > 0)
            {
                theReturn = theReturn + $"[{string.Join(",", Enumerable.Repeat(string.Empty, ArrayDimensions))}]";
            }

            if (prefixWithGlobal)
            {
                theReturn = "global::" + theReturn;
            }
            return theReturn;

        }
        public string GenerateReflectionName()
        {
            string theReturn = GenerateReflectionNameWithoutGenericsAndArrays();
            if (IsGenericTypeDefinition)
            {
                if (GenericParameters.IsNullOrEmpty())
                {
#if DEBUG
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                    else
                    {
                        System.Diagnostics.Debugger.Launch();
                    }
#else
                throw new InvalidOperationException($"the type {theReturn} is marked as IsGenericTypeDefinition but it's missing its GenericParameters types");
#endif
                }
                theReturn = $"{theReturn}`{GenericParameters.Length}";
            }
            else if (IsConstructedGenericType)
            {
                if (Wrapped.IsNullOrEmpty())
                {
#if DEBUG
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                    else
                    {
                        System.Diagnostics.Debugger.Launch();
                    }
#else
                throw new InvalidOperationException($"the type {theReturn} is marked as IsConstructedGenericType but it's missing its Wrapped types ");
#endif
                }
                theReturn = $"{theReturn}`{Wrapped.Length}[{string.Join(",", Wrapped.Select(t => $"[{t.GenerateReflectionName()}]"))}]";
            }

            if (ArrayDimensions > 0)
            {
                return theReturn + $"[{string.Join(",", Enumerable.Repeat(string.Empty, ArrayDimensions))}]";
            }
            else
            {
                return theReturn;
            }
        }
        public override string ToString()
        {
            return GenerateName();
        }
        public TypeRef GetNonArrayType()
        {
            var theReturn = Clone();
            theReturn.ArrayDimensions = 0;
            return theReturn;
        }

        public HashSet<TypeRef> SelfAndReferencedNames() => AddSelfAndReferencedTypeRefs(new HashSet<TypeRef>(new ObjectReferenceEqualityComparer<TypeRef>()));

        public HashSet<TypeRef> AddSelfAndReferencedTypeRefs(HashSet<TypeRef> existingList)
        {
            if (!existingList.Add(this))
            {
                return existingList;
            }

            if (NestedIn != null)
            {
                NestedIn.AddSelfAndReferencedTypeRefs(existingList);
            }

            foreach (var currentWrapped in Wrapped.EmptyIfNull())
            {
                currentWrapped.AddSelfAndReferencedTypeRefs(existingList);
            }
            return existingList;
        }

        #region "Cloning and Equality"
        public TypeRef Clone() => new TypeRef()
        {
            ArrayDimensions = ArrayDimensions,
            GenericParameters = GenericParameters?.Select(gp => gp.Clone()).ToArray(),
            IsPrimitive = IsPrimitive,
            IsConstructedGenericType = IsConstructedGenericType,
            IsEnum = IsEnum,
            IsGenericParameter = IsGenericParameter,
            IsGenericTypeDefinition = IsGenericTypeDefinition,
            IsWellKnown = IsWellKnown,
            Name = Name,
            Namespace = Namespace,
            NestedIn = NestedIn?.Clone(),
            Wrapped = Wrapped?.Select(w => w.Clone()).ToArray(),
            IsInterface = IsInterface
        };
        object ICloneable.Clone() => Clone();
        bool IEquatable<TypeRef>.Equals(TypeRef other) => Equals(other);

        public static bool operator ==(TypeRef a, TypeRef b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return (ReferenceEquals(a, null) && ReferenceEquals(b, null));
            }
            else
            {
                if (ReferenceEquals(a, b))
                {
                    return true;
                }
                else
                {
                    return a.GenerateName() == b.GenerateName();
                }
            }
        }

        public static bool operator !=(TypeRef a, TypeRef b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return GenerateName().GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            else
            {
                if (typeof(TypeRef) == obj.GetType())
                {
                    return this == (TypeRef)obj;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool Equals(TypeRef other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            else
            {
                return this == other;
            }
        }

        #endregion

    }
}
