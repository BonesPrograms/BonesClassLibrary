using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;

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

    Array
}

/// <summary>
/// Multiton wrapper for a metadata object. Primarily exists to return readable and informative strings about the metadata.
/// </summary>

public sealed class Metadata : MetadataReader
{
    /// <summary>
    /// Cache of all Metadata instances, to preserve multiton behavior.
    /// Module + MetadataToken keys are preferred to using MemberInfo instances here (more accurate).
    /// </summary>
    static readonly ConcurrentDictionary<(Module, int), Metadata> Cache = [];
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
    Metadata(MemberInfo info) : base(info)
    {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(bytes, info.MetadataToken);
        ByteToken = [.. bytes];
        MetadataType = GetMetadataType(info);
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
        if (type.IsEnum)
            return MetadataType.Enum;
        else if (type.IsArray)
            return MetadataType.Array;
        else if (type.IsClass)
            return MetadataType.Class;
        else if (type.IsInterface)
            return MetadataType.Interface;
        else
            return MetadataType.Struct;
    }

    //i may add access modifiers - it would be easy, just put my access modifiers right before we call base.tostring()
    //may also want to add other clarifiers like if they are abstract, sealed, virtual, new, etc
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(base.ToString());
        sb.Append($" Token:: {IntToken}");
        sb.Append(" AsBytes:: ");
        foreach (var bits in ByteToken)
        {
            sb.Append($"{bits} ");
        }
        return sb.ToString();
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
        return Cache.GetOrAdd((info.Module, info.MetadataToken), val => new(info));
    }

    /// <summary>
    /// Maps all the metadata in a module to a dictionary, keyed by type with a list of all the type's members in metadata format.
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public static Dictionary<Metadata, List<Metadata>> MetadataMap(Module m)
    {
        Dictionary<Metadata, List<Metadata>> metadata = [];
        ResolveAllMetadata(metadata, m.GetTypes());
        return metadata;

    }

    /// <summary>
    /// Checks if this metadata represents a specific metadata object.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public bool Represents(MemberInfo info)
    {
        return Info.HasSameMetadataDefinitionAs(info);
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

    public static bool operator ==(Metadata? data1, Metadata? data2)
    {
        return data1?.Equals(data2?.Info) ?? data2 == null;
    }

    public static bool operator !=(Metadata? data1, Metadata? data2)
    {
        return !(data1 == data2);
    }
    public override bool Equals(object? obj)
    {
        if (obj is MemberInfo info)
            return Represents(info);
        return false;
    }

    const BindingFlags Default = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    static void ResolveAllMetadata(Dictionary<Metadata, List<Metadata>> metadata, Type[] types)
    {
        foreach (var type in types)
        {
            List<Metadata> typedata = [];
            GetMetadataOf(typedata, type.GetProperties(Default));
            GetMetadataOf(typedata, type.GetFields(Default));
            GetMetadataOf(typedata, type.GetMethods(Default));
            GetMetadataOf(typedata, type.GetConstructors(Default));
            GetMetadataOf(typedata, type.GetEvents(Default));
            //  GetMetadataOf(metadata, type.GetMembers(Default));
            metadata[new(type)] = typedata;
        }
    }

    static void GetMetadataOf<T>(List<Metadata> metadata, T[] array) where T : MemberInfo
    {
        foreach (T obj in array)
        {
            metadata.Add(Get(obj));
        }
    }
}