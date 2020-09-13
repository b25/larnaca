using System.Collections.Generic;

namespace Larnaca.Schematics
{
    /// <summary>
    /// Generates type ids that are identical for IsConstructedGenericType, IsGenericTypeDefinition and Arrays
    /// To be used to lookup type outlines of a type definition.
    /// e.g. Dictionary<,> Dictionary<int,string> and Dictionary<int,string>[,] all reference the same dictionary type: Dictionary
    /// the type id for all of them will be System.Collections.Generic.Dictionary'2
    /// </summary>
    public class AproxTypeRefComparer : IEqualityComparer<TypeRef>
    {
        public static string GenerateTypeId(TypeRef name)
        {
            if (name == null)
            {
                return null;
            }
            string theReturn;
            if (name.NestedIn != null)
            {
                theReturn = $"{GenerateTypeId(name.NestedIn)}.{name.Name}";
            }
            else if (!string.IsNullOrWhiteSpace(name.Namespace))
            {
                theReturn = $"{name.Namespace}.{name.Name}";
            }
            else
            {
                theReturn = name.Name;
            }

            if (name.IsGenericTypeDefinition)
            {
                theReturn = $"{theReturn}'{name.GenericParameters.Length}";
            }
            else if (name.IsConstructedGenericType)
            {
                theReturn = $"{theReturn}'{name.Wrapped.Length}";
            }
            return theReturn;
        }
        public static bool Equals(TypeRef x, TypeRef y)
        {
            return GenerateTypeId(x) == GenerateTypeId(y);
        }
        public static int GetHashCode(TypeRef obj)
        {
            return GenerateTypeId(obj)?.GetHashCode() ?? 0;
        }
        bool IEqualityComparer<TypeRef>.Equals(TypeRef x, TypeRef y) => Equals(x, y);

        int IEqualityComparer<TypeRef>.GetHashCode(TypeRef obj) => GetHashCode(obj);
    }
}
