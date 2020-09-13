using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Larnaca.Schematics
{
    public static class PredefinedTypeRefs
    {
        public static TypeRef Boolean { get; } = Describer.Default.GetTypeRef(typeof(System.Boolean));
        public static TypeRef Byte { get; } = Describer.Default.GetTypeRef(typeof(System.Byte));
        public static TypeRef SByte { get; } = Describer.Default.GetTypeRef(typeof(System.SByte));
        public static TypeRef Char { get; } = Describer.Default.GetTypeRef(typeof(System.Char));
        public static TypeRef Decimal { get; } = Describer.Default.GetTypeRef(typeof(System.Decimal));
        public static TypeRef Double { get; } = Describer.Default.GetTypeRef(typeof(System.Double));
        public static TypeRef Single { get; } = Describer.Default.GetTypeRef(typeof(System.Single));
        public static TypeRef Int32 { get; } = Describer.Default.GetTypeRef(typeof(System.Int32));
        public static TypeRef UInt32 { get; } = Describer.Default.GetTypeRef(typeof(System.UInt32));
        public static TypeRef Int64 { get; } = Describer.Default.GetTypeRef(typeof(System.Int64));
        public static TypeRef UInt64 { get; } = Describer.Default.GetTypeRef(typeof(System.UInt64));
        public static TypeRef Int16 { get; } = Describer.Default.GetTypeRef(typeof(System.Int16));
        public static TypeRef UInt16 { get; } = Describer.Default.GetTypeRef(typeof(System.UInt16));

        public static TypeRef Guid { get; } = Describer.Default.GetTypeRef(typeof(System.Guid));
        public static TypeRef DateTime { get; } = Describer.Default.GetTypeRef(typeof(DateTime));
        public static TypeRef Timespan { get; } = Describer.Default.GetTypeRef(typeof(TimeSpan));
        public static TypeRef DayOfWeek { get; } = Describer.Default.GetTypeRef(typeof(System.DayOfWeek));
        public static TypeRef DateTimeOffset { get; } = Describer.Default.GetTypeRef(typeof(System.DateTimeOffset));
        public static TypeRef DateTimeKind { get; } = Describer.Default.GetTypeRef(typeof(System.DateTimeKind));

        public static TypeRef Object { get; } = Describer.Default.GetTypeRef(typeof(System.Object));
        public static TypeRef String { get; } = Describer.Default.GetTypeRef(typeof(System.String));
        public static TypeRef Hashtable { get; } = Describer.Default.GetTypeRef(typeof(System.Collections.Hashtable));

        public static TypeRef List { get; } = Describer.Default.GetTypeRef(typeof(List<>));
        public static TypeRef Dictionary { get; } = Describer.Default.GetTypeRef(typeof(Dictionary<,>));
        public static TypeRef HashSet { get; } = Describer.Default.GetTypeRef(typeof(HashSet<>));
        public static TypeRef IEnumerable { get; } = Describer.Default.GetTypeRef(typeof(System.Collections.IEnumerable));
        public static TypeRef GenericIEnumerable { get; } = Describer.Default.GetTypeRef(typeof(IEnumerable<>));
        public static TypeRef ICollection { get; } = Describer.Default.GetTypeRef(typeof(ICollection<>));
        public static TypeRef IList { get; } = Describer.Default.GetTypeRef(typeof(IList<>));
        public static TypeRef GenericNullable { get; } = Describer.Default.GetTypeRef(typeof(Nullable<>));
        public static TypeRef KeyValuePair { get; } = Describer.Default.GetTypeRef(typeof(KeyValuePair<,>));
        public static TypeRef Tuple { get; } = Describer.Default.GetTypeRef(typeof(Tuple<,>));
        public static TypeRef ValueTuple { get; } = Describer.Default.GetTypeRef(typeof(ValueTuple<,>));
        public static TypeRef Stream { get; } = Describer.Default.GetTypeRef(typeof(System.IO.Stream));
        public static TypeRef[] AllPredefinedTypes { get; } = new TypeRef[]
        {
            Boolean,
            Byte,
            SByte,
            Char,
            Decimal,
            Double,
            Single,
            Int32,
            UInt32,
            Int64,
            UInt64,
            Int16,
            UInt16,
            Guid,
            DateTime,
            Timespan,
            DayOfWeek,
            DateTimeOffset,
            DateTimeKind,
            Object,
            String,
            Hashtable,
            List,
            Dictionary,
            HashSet,
            IEnumerable,
            GenericIEnumerable,
            ICollection,
            IList,
            GenericNullable,
            KeyValuePair,
            Tuple,
            ValueTuple,
            Stream
        };

        // TODO: replace with HashSet when netstandard2.1 is released with the HashSet<T>.TryGetValue implemented
        public static Dictionary<TypeRef, TypeRef> AllPredefinedTypesDefaultComparison { get; } = AllPredefinedTypes.ToDictionary(t => t, t => t);
        // TODO: replace with HashSet when netstandard2.1 is released with the HashSet<T>.TryGetValue implemented
        public static Dictionary<TypeRef, TypeRef> AllPredefinedTypesAproxComparison { get; } = new Dictionary<TypeRef, TypeRef>(AllPredefinedTypesDefaultComparison, new AproxTypeRefComparer());

        public static TypeRef[] PrimitiveTypes { get; } = new TypeRef[]
        {
            Boolean,
            Byte,
            SByte,
            Char,
            Decimal,
            Double,
            Single,
            Int32,
            UInt32,
            Int64,
            UInt64,
            Object,
            Int16,
            UInt16
        };

        // TODO: replace with HashSet when netstandard2.1 is released with the HashSet<T>.TryGetValue implemented
        public static Dictionary<TypeRef, TypeRef> PrimitiveTypesDefaultComparison { get; } = PrimitiveTypes.ToDictionary(t => t, t => t);
        public static Dictionary<TypeRef, TypeRef> PrimitiveTypesAproxComparison { get; } = new Dictionary<TypeRef, TypeRef>(PrimitiveTypesDefaultComparison, new AproxTypeRefComparer());
        public static TypeRef[] AllPredefinedAndNullableTypes { get; } = AllPredefinedTypes.Union(Nullable.AllPredefinedNullableTypes).ToArray();
        // TODO: replace with HashSet when netstandard2.1 is released with the HashSet<T>.TryGetValue implemented
        public static Dictionary<TypeRef, TypeRef> AllPredefinedAndNullableTypesDefaultComparison { get; } = AllPredefinedAndNullableTypes.ToDictionary(t => t, t => t);


        public static class Nullable
        {
            public static TypeRef Boolean { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.Boolean>));
            public static TypeRef Byte { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.Byte>));
            public static TypeRef SByte { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.SByte>));
            public static TypeRef Char { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.Char>));
            public static TypeRef Decimal { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.Decimal>));
            public static TypeRef Double { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.Double>));
            public static TypeRef Single { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.Single>));
            public static TypeRef Int32 { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.Int32>));
            public static TypeRef UInt32 { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.UInt32>));
            public static TypeRef Int64 { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.Int64>));
            public static TypeRef UInt64 { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.UInt64>));
            public static TypeRef Int16 { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.Int16>));
            public static TypeRef UInt16 { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.UInt16>));

            public static TypeRef Guid { get; } = Describer.Default.GetTypeRef(typeof(Nullable<System.Guid>));
            public static TypeRef DateTime { get; } = Describer.Default.GetTypeRef(typeof(Nullable<DateTime>));
            public static TypeRef Timespan { get; } = Describer.Default.GetTypeRef(typeof(Nullable<TimeSpan>));

            public static TypeRef[] PrimitiveNullableTypes { get; } = new TypeRef[]
            {
                Boolean,
                Byte,
                SByte,
                Char,
                Decimal,
                Double,
                Single,
                Int32,
                UInt32,
                Int64,
                UInt64,
                Int16,
                UInt16
            };

            // TODO: replace with HashSet when netstandard2.1 is released with the HashSet<T>.TryGetValue implemented
            public static Dictionary<TypeRef, TypeRef> PrimitiveNullableTypesDefaultComparison { get; } = PrimitiveNullableTypes.ToDictionary(t => t, t => t);

            // approx comparison is unapplicable for nullable because aprox. comparison will match Nullable<> Nullable to any other Nullable
            public static TypeRef[] AllPredefinedNullableTypes { get; } = new TypeRef[]
            {
                Boolean,
                Byte,
                SByte,
                Char,
                Decimal,
                Double,
                Single,
                Int32,
                UInt32,
                Int64,
                UInt64,
                Int16,
                UInt16,
                Guid,
                DateTime,
                Timespan,
            };

            // TODO: replace with HashSet when netstandard2.1 is released with the HashSet<T>.TryGetValue implemented
            public static Dictionary<TypeRef, TypeRef> AllPredefinedNullableTypesDefaultComparison { get; } = AllPredefinedNullableTypes.ToDictionary(t => t, t => t);

            // approx comparison is unapplicable for nullable because aprox. comparison will match Nullable<> Nullable to any other Nullable
        }
    }
}
