using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Bones.Collections;

namespace BonesClassLibrary.Reflection;


public enum MetadataType
{
    Member,
    Class,
    Struct,
    Interface,
    Field,
    Property,
    Method,
    Event,
    Constructor,
    Enum,
    Array,
    Collection,
    Delegate
}

/// <summary>
/// Multiton wrapper for a metadata object. Primarily exists to return readable and informative strings about the metadata.
/// </summary>

public sealed class Metadata : MetadataReader
{
    public MemberInfo Info => (MemberInfo)Object!;
    public Module Module => Info.Module;
    public int IntToken => Info.MetadataToken;
    public string Name => Info.Name; //may change this to be info.tostring() for type objects (so you can see the namespace), not sure how that will effect methodinfo objects tho
    public readonly MetadataType MetadataType; //mostly for querying - you have options, can search metadata maps by metadatatype/name, or can search them by their info object
                                               //using the method Metadata.Represents(MemberInfo)
    /// <summary>
    /// 32bit byte sequence of the MetadataToken.
    /// </summary>
    public readonly ImmutableArray<byte> ByteToken;
    readonly Type? Declared;
    readonly Type? Base;
    Metadata(MemberInfo info) : base(info)
    {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(bytes, info.MetadataToken);
        ByteToken = [.. bytes];
        MetadataType = GetMetadataType(info);
        Declared = info.DeclaringType;
        if (info is Type type)
            Base = type.BaseType;
    }


    /// these methods are helpers for navigating a MetadataMap -
    /// members are returned following their type so if you want to get a class's data you can use IsOrIsDeclaredIn
    public bool EqualsOrIsDeclaredIn(Type type)
    {
        return this == type || DeclaredIn(type);
    }

    public bool EqualsOrIsDeclaredIn(Metadata data)
    {
        return this == data || DeclaredIn(data);
    }
    public bool DeclaredIn(Metadata data)
    {
        return DeclaredIn(data.Declared);
    }
    public bool DeclaredIn(Type? type)
    {
        if (type == null)
            return false;
        return Declared?.HasSameMetadataDefinitionAs(type) ?? false;
    }

    /// <summary>
    /// Get metadata from a class object. Can also be used to wrap MemberInfo objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Metadata Get<T>(T obj) where T : class
    {
        MemberInfo info = obj is MemberInfo inf ? inf : obj.GetType();
        return new(info);
    }

    /// <summary>
    /// Maps all the metadata in a module to a dictionary, keyed by type with a list of all the type's members in metadata format.
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public static MetadataMap MetadataMap(Module m)
    {
        Dictionary<Metadata, ImmutableList<Metadata>> metadata = [];
        ResolveAllMetadata(metadata, m.GetTypes());
        return new MetadataMap(metadata);

    }
    /// <summary>
    /// Casts the memberinfo object to whatever descendant class you need.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? CastInfoTo<T>() where T : MemberInfo => Info as T;

    /// <summary>
    /// Casts the memberinfo object to whatever descendant class you need.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool CastInfoTo<T>(out T? obj) where T : MemberInfo
    {
        obj = CastInfoTo<T>();
        return obj != null;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Info);
    }


    //The Equals override for the Metadata class compares the MemberInfo object that it wraps. It compares them by metadata definition rather
    //than object reference. Additionally, the Metadata class can be compared to a MemberInfo object, and will be evaluated in the same manner.

    ///Metadata == memberinfo // vice versa

    public static bool operator !=(MemberInfo? info, Metadata? data) => !(info == data);

    public static bool operator ==(MemberInfo? info, Metadata? data) => data == info;

    public static bool operator !=(Metadata? data, MemberInfo? info) => !(data == info);

    public static bool operator ==(Metadata? data, MemberInfo? info)
    {
        return data?.Equals(info) ?? info == null;
    }

    //Metadata == Metadta
    public static bool operator !=(Metadata? data1, Metadata? data2) => !(data1 == data2);

    public static bool operator ==(Metadata? data1, Metadata? data2)
    {
        return data1?.Equals(data2?.Info) ?? data2 is null;
    }

    public override bool Equals(object? obj)
    {
        if (obj is MemberInfo info)
            return info.HasSameMetadataDefinitionAs(Info);
        return false;
    }
    //i may add access modifiers - it would be easy, just put my access modifiers right before we call base.tostring()
    //may also want to add other clarifiers like if they are abstract, sealed, virtual, new, etc
    ///will require lots of separate methods and casting, cannot do this like i did MemberToString
    /// other stuff like checking if a metadata is static or instance, such as if a class or property or field is static
    /// maybe the word "method" before methods, "constructor" before constructors
    /// maybe also get generic constraits too, like where T : Memberinfo
    /// 
    protected override StringBuilder ToStringBuilder()
    {
        StringBuilder sb = new();
        sb.Append(MetadataTypeToString());
        sb.Append(base.ToStringBuilder());
        if (Base != null)
        {
            sb.Append(" : ");
            sb.Append(CheckTypeGenerics(Base));
        }
        sb.Append($" Token:: {IntToken}");
        sb.Append(" AsBytes:: ");
        foreach (var bits in ByteToken)
        {
            sb.Append($"{bits} ");
        }
        return sb;
    }

    static void ResolveAllMetadata(Dictionary<Metadata, ImmutableList<Metadata>> metadata, Type[] types)
    {
        const BindingFlags allDeclared = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        foreach (var type in types)
        {
            List<Metadata> typedata = [];
            GetMetadataOf(typedata, type.GetProperties(allDeclared));
            GetMetadataOf(typedata, type.GetFields(allDeclared));
            GetMetadataOf(typedata, type.GetMethods(allDeclared));
            GetMetadataOf(typedata, type.GetConstructors(allDeclared));
            GetMetadataOf(typedata, type.GetEvents(allDeclared));
            //  GetMetadataOf(metadata, type.GetMembers(Default));
            metadata[new(type)] = [.. typedata];
        }
    }

    static void GetMetadataOf<T>(List<Metadata> metadata, T[] array) where T : MemberInfo
    {
        foreach (T obj in array)
        {
            metadata.Add(Get(obj));
        }
    }


    StringBuilder? MetadataTypeToString() => Info switch
    {
        Type => TypeToString((Type)Info),
        MethodInfo or ConstructorInfo => MethodToString((MethodBase)Info),
        FieldInfo => FieldToString((FieldInfo)Info),
        _ => null, //lol i never actually used events so im gonna learn them before i start reflecting them
    };

    //ive found this currently isnt necesary because the actual get and setter methods are already being read with their access modifiers shown
    //though it could use a bit more organization, prob will have it find the getters and setters by name get_ set_ and then shift them up to be below their
    //respective property in the list, based on the actual name of the property

    //also this shit wasnt even really working anyways lmao

    // static StringBuilder PropertyToString(PropertyInfo prop)
    // {
    //     StringBuilder sb = new();
    //     MethodInfo? get = prop.GetGetMethod();
    //     if (get != null)
    //     {
    //         sb.Append(MethodToString(get));
    //         sb.Append("get ");
    //     }
    //     MethodInfo? set = prop.GetSetMethod();
    //     if (set != null)
    //     {
    //         sb.Append(MethodToString(set));
    //         sb.Append("set ");
    //     }
    //     return sb;
    // }

    static StringBuilder FieldToString(FieldInfo field)
    {
        StringBuilder sb = new();
        AccessModifiers(sb, new AccessModifiers(field));
        if (field.IsStatic && field.IsLiteral)
            sb.Append("static ");
        else if (field.IsLiteral)
            sb.Append("const ");
        return sb;

    }

    static StringBuilder MethodToString(MethodBase mthd)
    {
        StringBuilder sb = new();
        AccessModifiers(sb, new AccessModifiers(mthd));
        if (mthd.IsStatic)
        {
            sb.Append("static ");
            return sb;
        }
        bool isoverride = false;
        if (mthd is MethodInfo realmethod)
        {
            if (realmethod.DeclaringType != realmethod.GetBaseDefinition().DeclaringType)
            {
                sb.Append("override ");
                isoverride = true;
            }
        }
        if (mthd.IsFinal)
            sb.Append("sealed ");
        else if (mthd.IsAbstract)
            sb.Append("abstract ");
        else if (!isoverride && mthd.IsVirtual)
            sb.Append("virtual ");
        return sb;
    }

    static void AccessModifiers(StringBuilder sb, AccessModifiers access)
    {
        if (access.IsPublic)
            sb.Append("public ");
        else if (access.IsFamily)
            sb.Append("protected ");
        else if (access.IsPrivate)
            sb.Append("private ");
        else if (access.IsAssembly)
            sb.Append("internal ");
        else if (access.IsFamilyAndAssembly)
            sb.Append("private protected ");
        else if (access.IsFamilyOrAssembly)
            sb.Append("protected internal ");
    }
    static StringBuilder TypeToString(Type type) //need to add stuff for nested types i think
    {
        StringBuilder sb = new();
        if (type.IsAbstract && type.IsSealed)
            sb.Append("static ");
        else if (type.IsAbstract)
            sb.Append("abstract ");
        else if (type.IsSealed)
            sb.Append("sealed ");
        return sb;
    }

    static MetadataType GetMetadataType(MemberInfo info) => info switch
    {
        ConstructorInfo => MetadataType.Constructor,
        PropertyInfo => MetadataType.Property,
        FieldInfo => MetadataType.Field,
        Type => GetMetadataTypeFromTypeObject((Type)info),
        EventInfo => MetadataType.Event,
        MethodInfo => MetadataType.Method,
        _ => MetadataType.Member

    };

    static MetadataType GetMetadataTypeFromTypeObject(Type type)
    {
        if (typeof(Delegate).IsAssignableFrom(type))
            return MetadataType.Delegate;
        if (type.IsEnum)
            return MetadataType.Enum;
        else if (type.IsArray)
            return MetadataType.Array;
        else if (type.IsInterface)
            return MetadataType.Interface;
        else if (type != typeof(string) && (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) || typeof(System.Collections.ICollection).IsAssignableFrom(type)))
            return MetadataType.Collection;
        else if (type.IsClass)
            return MetadataType.Class;
        else
            return MetadataType.Struct;
    }
}