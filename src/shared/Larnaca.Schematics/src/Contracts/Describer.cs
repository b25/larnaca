using Larnaca.Blueprints;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Larnaca.Schematics
{
    public class Describer
    {
        public static readonly string[] ValidTaskTypeRefs = new string[] { "ValueTask`1", "Task`1" };
        public static readonly Describer Default = new Describer();
        public Assembly[] WellKnownAssemblies { get; set; }

        private object cacheLocker = new object();
        private Dictionary<TypeRef, Type> typesByNameCache = new Dictionary<TypeRef, Type>();
        //private Dictionary<Type, TypeRef> namesByTypeCache = new Dictionary<Type, TypeRef>();
        //private Dictionary<Type, TypeOutline> outlinesByType = new Dictionary<Type, TypeOutline>();
        //private Dictionary<TypeOutline, Type> typesByOutline = new Dictionary<TypeOutline, Type>();
        //private Dictionary<TypeRef, TypeOutline> outlinesByName = new Dictionary<TypeRef, TypeOutline>(new AproxTypeRefComparer());

        public Describer()
        {
            WellKnownAssemblies = new Assembly[] { typeof(object).Assembly, typeof(System.Collections.Generic.HashSet<>).Assembly };
        }
        public Describer(Assembly[] wellKnownAssemblies)
        {
            WellKnownAssemblies = wellKnownAssemblies;
        }
        //public OperationResult<TypeOutline> GetWellKnownTypeOutline(Model model, TypeRef typeName)
        //{
        //    Type type;
        //    lock (cacheLocker)
        //    {
        //        typesByNameCache.TryGetValue(typeName, out type);
        //    }
        //    if (type == null)
        //    {
        //        type = Type.GetType(typeName.GenerateReflectionName(), false);
        //    }
        //    if (type == null)
        //    {
        //        return new OperationResult<TypeOutline>($"Failed to find the WellKnown type {typeName.GenerateName()}. Apparently it's not known all that well");
        //    }
        //    return OutlineTypeAndReferencedTypesAddToModel(model, type, typeName, true).ToOperationResult();
        //}

        public Model GetModelFromAssembly(string assemblyName)
        {
            System.Diagnostics.Debugger.Launch();
            var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains(assemblyName)) ?? Assembly.Load(assemblyName);
            var types = asm?.GetTypes();
            return null;
        }

        public Model DescribeType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return DescribeTypes(new Type[] { type });
        }

        public Model DescribeTypes(IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            return DescribeTypes(new Model(), types);
        }
        public Model DescribeType(Model model, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return DescribeTypes(model, new Type[] { type });
        }
        public Model DescribeTypes(Model model, IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
            foreach (var currentType in types)
            {
                OutlineTypeAndReferencedTypesAddToModel(model, currentType);
            }

            return model;
        }


        public TypeRef GetTypeRef(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            TypeRef theReturn;
            //lock (cacheLocker)
            //{
            //    if (namesByTypeCache.TryGetValue(type, out theReturn))
            //    {
            //        return theReturn;
            //    }
            //}
            if (type.IsArray)
            {
                var wrappedType = GetTypeRef(type.GetElementType());
                theReturn = wrappedType.Clone();
                theReturn.ArrayDimensions = type.GetArrayRank();
            }
            else
            {
                theReturn = new TypeRef()
                {
                    Namespace = type.Namespace,
                    IsConstructedGenericType = type.IsConstructedGenericType && !type.IsEnum,
                    IsGenericTypeDefinition = type.IsGenericTypeDefinition && !type.IsEnum,
                    IsGenericParameter = type.IsGenericParameter && !type.IsEnum,
                    IsEnum = type.IsEnum,
                    IsInterface = type.IsInterface
                };
                var indexOfAccent = type.Name.IndexOf('`');
                if (indexOfAccent > 0)
                {
                    theReturn.Name = type.Name.Substring(0, indexOfAccent);
                }
                else
                {
                    theReturn.Name = type.Name;
                }

                if (type.IsNested && !type.IsGenericParameter)
                {
                    theReturn.NestedIn = GetTypeRef(type.DeclaringType);
                }

                if (type.IsGenericTypeDefinition)
                {
                    theReturn.GenericParameters = type.GetGenericArguments().Select(t => GetTypeRef(t)).ToArray();
                }
                else if (type.IsConstructedGenericType)
                {
                    theReturn.Wrapped = type.GetGenericArguments().Select(GetTypeRef).ToArray();
                }

                theReturn.IsPrimitive = PrimitiveTypes.Contains(type);
                theReturn.IsWellKnown = WellKnownAssemblies.Contains(type.Assembly);
            }
            lock (cacheLocker)
            {
                typesByNameCache[theReturn] = type;
                //namesByTypeCache[type] = theReturn;
            }
            return theReturn;
        }
        public TypeRef GetTypeRefAddOutlineToModel(Model model, Type type)
        {
            var theReturn = GetTypeRef(type);
            OutlineTypeAndReferencedTypesAddToModel(model, type, theReturn);
            return theReturn;
        }
        public TypeRef GetNameAddWithReferencesToModel(Model model, Type type, bool forceOutlineWellKnown = false)
        {
            var name = GetTypeRef(type);
            OutlineTypeAndReferencedTypesAddToModel(model, type, name, forceOutlineWellKnown);
            return name;
        }
        public (TypeRef name, TypeOutline outline) GetOutlineAndNameAddWithReferencesToModel(Model model, Type type, bool forceOutlineWellKnown = false)
        {
            var name = GetTypeRef(type);
            var outline = OutlineTypeAndReferencedTypesAddToModel(model, type, name, forceOutlineWellKnown);
            return (name, outline);
        }
        private TypeOutline OutlineTypeAndReferencedTypesAddToModel(Model model, Type type, TypeRef typeName = null, bool forceOutlineWellKnown = false)
        {
            TypeOutline theReturn;
            TypeRef typeNameToUse;
            // TODID: cache disabled
            //bool isInOutlinesByType;
            //bool isInOutlinesByName;
            //lock (cacheLocker)
            //{
            //    isInOutlinesByType = outlinesByType.TryGetValue(type, out theReturn);
            //}
            //// TODO!!!
            //// when getting a type from the cache, the refrenced types are not added to the model!!!
            //// this is a big problem, we need to either ditch the cache, or construct a web of type references when describing the initial type, or figure out the referenced types on each call
            //if (isInOutlinesByType)
            //{
            //    typeNameToUse = theReturn?.Name ?? typeName ?? GetTypeRef(type);
            //    isInOutlinesByName = true;
            //}
            //else
            //{
            //    // check the cache by type name
            //    // we need to check the cache again because some types will have the same outline
            //    // this happens for generic and array types
            //    // eaxmples of different types with the same outline:
            //    // Generics: Dictionary<int,string> and Dictionary<string,object> 
            //    // Array: Contract[], Contract
            //    typeNameToUse = typeName ?? GetTypeRef(type);
            //    //lock (cacheLocker)
            //    //{
            //    //    if (isInOutlinesByName = outlinesByName.TryGetValue(typeNameToUse, out theReturn))
            //    //    {
            //    //        outlinesByType[type] = theReturn;
            //    //    }
            //    //}
            //}

            typeNameToUse = typeName ?? GetTypeRef(type);

            //#if (DEBUG)
            //            if (typeNameToUse.Namespace.StartsWith("LJS"))
            //            {
            //                if (!System.Diagnostics.Debugger.IsAttached)
            //                {
            //                    System.Diagnostics.Debugger.Launch();
            //                }
            //            }
            //#endif
            if (model.Outlines.TryGetValue(typeNameToUse, out theReturn))
            {
                if (type.IsConstructedGenericType)
                {
                    foreach (var currentParam in type.GetGenericArguments().EmptyIfNull())
                    {
                        if (!currentParam.IsGenericParameter)
                        {
                            OutlineTypeAndReferencedTypesAddToModel(model, currentParam, forceOutlineWellKnown: forceOutlineWellKnown);
                        }
                    }
                }

                return theReturn;
            }

            if (typeNameToUse.IsGenericParameter)
            {
                theReturn = null;
            }
            else /*if (!isInOutlinesByType && !isInOutlinesByName)*/
            {
                if (!type.IsEnum && (type.IsArray || type.IsConstructedGenericType))
                {
                    // get the real underlying type                
                    if (type.IsArray)
                    {
                        theReturn = OutlineTypeAndReferencedTypesAddToModel(model, type.GetElementType(), forceOutlineWellKnown: forceOutlineWellKnown);
                    }
                    else // if (type.IsConstructedGenericType)
                    {
                        theReturn = OutlineTypeAndReferencedTypesAddToModel(model, type.GetGenericTypeDefinition(), forceOutlineWellKnown: forceOutlineWellKnown);
                        foreach (var currentParam in type.GetGenericArguments().EmptyIfNull())
                        {
                            if (!currentParam.IsGenericParameter)
                            {
                                OutlineTypeAndReferencedTypesAddToModel(model, currentParam, forceOutlineWellKnown: forceOutlineWellKnown);
                            }
                        }
                    }
                }
                else if (typeNameToUse.IsPrimitive || (typeNameToUse.IsWellKnown && !forceOutlineWellKnown))
                {
                    theReturn = null;
                    //lock (cacheLocker)
                    //{
                    //    outlinesByType[type] = null;
                    //    outlinesByName[typeNameToUse] = null;
                    //}
                }
                else
                {
                    // the real deal, we need to outline the type
                    theReturn = new TypeOutline()
                    {
                        Name = typeNameToUse
                    };

                    //lock (cacheLocker)
                    //{
                    //    outlinesByName[typeNameToUse] = theReturn;
                    //    outlinesByType[type] = theReturn;
                    //    typesByOutline[theReturn] = type;
                    //}
                    model.Outlines[typeNameToUse] = theReturn;

                    var protoContractAttr = type.GetCustomAttribute<ProtoContractAttribute>(false);
                    var dataCotnractAttr = type.GetCustomAttribute<DataContractAttribute>(true);
                    var knownTypeAttrs = type.GetCustomAttributes<KnownTypeAttribute>(false);
                    var protoIncludeAtts = type.GetCustomAttributes<ProtoIncludeAttribute>(false);

                    if (null != (protoContractAttr
                        ?? dataCotnractAttr
                        ?? knownTypeAttrs
                        ?? (object)protoIncludeAtts))
                    {
                        theReturn.SerializationDetails = new TypeSerializationDetails();

                        if (protoContractAttr != null)
                        {
                            theReturn.SerializationDetails.ProtoContractDetails = new ProtoContractDetails(model, this, protoContractAttr);
                        }

                        if (dataCotnractAttr != null)
                        {
                            theReturn.SerializationDetails.DataContractDetails = new DataContractDetails(dataCotnractAttr);
                        }

                        if (knownTypeAttrs.IsAny())
                        {
                            theReturn.SerializationDetails.KnownTypeItems = knownTypeAttrs.Select(attr => new KnownTypeItem(model, this, attr)).ToArray();
                        }

                        if (protoIncludeAtts.IsAny())
                        {
                            theReturn.SerializationDetails.ProtoIncludeItems = protoIncludeAtts.Select(attr => new ProtoIncludeItem(model, this, attr)).ToArray();
                        }
                    }

                    if (type.IsEnum)
                    {
                        theReturn.Classification = TypeClassification.Enum;
                        theReturn.Base = GetTypeRef(Enum.GetUnderlyingType(type));
                    }
                    else if (type.IsClass)
                    {
                        theReturn.Classification = TypeClassification.Class;
                    }
                    else if (type.IsInterface)
                    {
                        theReturn.Classification = TypeClassification.Interface;
                    }
                    else if (type.IsValueType)
                    {
                        theReturn.Classification = TypeClassification.Struct;
                    }

                    List<TypeMember> outlinedMembers = new List<TypeMember>();
                    MemberInfo[] members;

                    if (theReturn.Classification == TypeClassification.Enum)
                    {
                        members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
                    }
                    else
                    {
                        members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    }

                    foreach (var currentMember in members)
                    {
                        TypeMember currentOutlinedMember = new TypeMember()
                        {
                            Name = currentMember.Name,
                            //DeclaringType = GetTypeRef(currentMember.DeclaringType)
                        };

                        if (theReturn.Classification == TypeClassification.Enum)
                        {
                            if (currentMember.Name != "value__")
                            {
                                EnumMemberAttribute enumMemberAttribute = null;
                                //if (LJSDescriber.TryGetEnumMember(currentMember, out enumMemberAttribute))
                                //{
                                //    // TODO add serialization details for enum members
                                //}

                                currentOutlinedMember.EnumValue = Convert.ToDecimal(Enum.Parse(type, currentMember.Name, false));
                                outlinedMembers.Add(currentOutlinedMember);
                            }
                        }
                        else
                        {
                            var dataMemberAttribute = currentMember.GetCustomAttribute<DataMemberAttribute>(true);
                            var ignoreDataMemberAttribute = currentMember.GetCustomAttribute<IgnoreDataMemberAttribute>(true);
                            ProtoMemberAttribute protoMemberAttribute;
                            bool hasInvalidProtoMemberAttribute;
                            try
                            {
                                protoMemberAttribute = currentMember.GetCustomAttribute<ProtoMemberAttribute>(true);
                                hasInvalidProtoMemberAttribute = false;
                            }
                            catch (System.ArgumentOutOfRangeException ex)
                            {
                                protoMemberAttribute = null;
                                hasInvalidProtoMemberAttribute = true;
                            }
                            var ignoreProtoMemberAttribute = currentMember.GetCustomAttribute<ProtoIgnoreAttribute>(true);

                            if (ignoreDataMemberAttribute != null || ignoreProtoMemberAttribute != null)
                            {
                                if (ignoreDataMemberAttribute != null && ignoreProtoMemberAttribute != null)
                                {
                                    continue;
                                }

                                if (protoMemberAttribute == null && dataMemberAttribute == null)
                                {
                                    // only one attribute specified: IgnoreDataMember or ProtoIgnore
                                    // so just ignore the member
                                    continue;
                                }
                            }



                            if (null != (dataMemberAttribute
                                ?? ignoreDataMemberAttribute
                                ?? protoMemberAttribute
                                ?? (object)ignoreDataMemberAttribute)
                                || hasInvalidProtoMemberAttribute)
                            {
                                currentOutlinedMember.SerializationDetails = new MemberSerializationDetails();
                                if (null != ((object)dataMemberAttribute ?? (object)ignoreDataMemberAttribute))
                                {
                                    currentOutlinedMember.SerializationDetails.DataMemberDetails = new DataMemberDetails();
                                    if (dataMemberAttribute != null)
                                    {
                                        currentOutlinedMember.SerializationDetails.DataMemberDetails.FillFromAttribute(dataMemberAttribute);
                                    }
                                    if (ignoreDataMemberAttribute != null)
                                    {
                                        currentOutlinedMember.SerializationDetails.DataMemberDetails.Ignore = true;
                                    }
                                }

                                if (null != ((object)protoMemberAttribute ?? ignoreProtoMemberAttribute) || hasInvalidProtoMemberAttribute)
                                {
                                    currentOutlinedMember.SerializationDetails.ProtoMemberDetails = new ProtoMemberDetails();
                                    if (protoMemberAttribute != null)
                                    {
                                        currentOutlinedMember.SerializationDetails.ProtoMemberDetails.FillFromAttribute(protoMemberAttribute);
                                    }
                                    if (ignoreProtoMemberAttribute != null)
                                    {
                                        currentOutlinedMember.SerializationDetails.ProtoMemberDetails.Ignore = true;
                                    }
                                    if (hasInvalidProtoMemberAttribute)
                                    {
                                        currentOutlinedMember.SerializationDetails.ProtoMemberDetails = new ProtoMemberDetails()
                                        {
                                            Tag = 0
                                        };
                                    }
                                }
                            }

                            Type memberType;
                            if (currentMember is FieldInfo currentField)
                            {
                                memberType = currentField.FieldType;
                            }
                            else if (currentMember is PropertyInfo currentProperty)
                            {
                                memberType = currentProperty.PropertyType;
                            }
                            else
                            {
                                // noop, we only care about fields and properties
                                memberType = null;
                            }

                            if (memberType != null)
                            {
                                TypeRef memberTypeRef = GetTypeRef(memberType);
                                currentOutlinedMember.Type = memberTypeRef;

                                outlinedMembers.Add(currentOutlinedMember);

                                var allReferencedTypes = memberTypeRef.SelfAndReferencedNames();
                                foreach (var currentReferencedTypeRef in allReferencedTypes)
                                {
                                    if (!currentReferencedTypeRef.IsPrimitive && !currentReferencedTypeRef.IsWellKnown && !currentReferencedTypeRef.IsGenericParameter)
                                    {
                                        Type currentReferencedType;
                                        lock (cacheLocker)
                                        {
                                            currentReferencedType = typesByNameCache[currentReferencedTypeRef];
                                        }
                                        OutlineTypeAndReferencedTypesAddToModel(model, currentReferencedType, currentReferencedTypeRef, forceOutlineWellKnown: forceOutlineWellKnown);
                                    }
                                }
                            }
                        }
                    }

                    if (typeNameToUse.IsGenericTypeDefinition)
                    {
                        var genericMembers = new Dictionary<TypeRef, List<string>>();
                        foreach (var currentMember in outlinedMembers)
                        {
                            if (currentMember.Type.IsGenericParameter)
                            {
                                genericMembers.AddToList(currentMember.Type, currentMember.Name);
                            }
                        }
                        theReturn.GenericMembers = genericMembers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
                    }

                    if (!type.IsEnum)
                    {
                        Type baseType = type.BaseType;
                        if (baseType != null && baseType != typeof(object) && baseType != typeof(ValueType))
                        {
                            theReturn.Base = GetNameAddWithReferencesToModel(model, baseType, forceOutlineWellKnown: forceOutlineWellKnown);
                        }

                        var implementedInterfaces = type.GetInterfaces();
                        if (implementedInterfaces.IsAny())
                        {
                            List<TypeRef> implementedInterfaceNames = new List<TypeRef>();
                            foreach (var currentInterface in implementedInterfaces)
                            {
                                var currentName = GetTypeRef(currentInterface);
                                implementedInterfaceNames.Add(currentName);
                                if (!currentName.IsWellKnown)
                                {
                                    OutlineTypeAndReferencedTypesAddToModel(model, currentInterface, currentName, forceOutlineWellKnown: forceOutlineWellKnown);
                                }
                            }
                            theReturn.ImplementedInterfaces = implementedInterfaceNames.ToArray();
                        }
                    }

                    if (typeNameToUse.NestedIn != null)
                    {
                        OutlineTypeAndReferencedTypesAddToModel(model, type.DeclaringType, forceOutlineWellKnown: forceOutlineWellKnown);
                    }

                    theReturn.Members = outlinedMembers.ToArray();
                    var asmName = type.Assembly.GetName();
                    theReturn.Assembly = $"{asmName.Name}@{asmName.Version}";

                }
            }

            if (theReturn != null && !model.Outlines.ContainsKey(typeNameToUse))
            {
                theReturn = theReturn.Clone();
                model.Outlines[typeNameToUse] = theReturn;
            }
            return theReturn;
        }

        public static readonly Type[] PrimitiveTypes = new Type[]
        {
            typeof(System.Boolean),
            typeof(System.Byte),
            typeof(System.SByte),
            typeof(System.Char),
            typeof(System.Decimal),
            typeof(System.Double),
            typeof(System.Single),
            typeof(System.Int32),
            typeof(System.UInt32),
            typeof(System.Int64),
            typeof(System.UInt64),
            typeof(System.Object),
            typeof(System.Int16),
            typeof(System.UInt16),
            typeof(System.String),
        };
    }
}
