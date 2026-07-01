using System.Reflection;
using BonesClassLibrary.Reflection;
using System.Collections.Immutable;
using System.Collections;

namespace BonesClassLibrary.Reflection.Collections;


/// <summary>
/// Wrapper class that simplifies enumerating a collection that is inside of a collection. Each key is a Type's GUID. Each array stores a Type object followed
/// by it's members.
/// </summary>

public sealed class MetadataMap : IEnumerable<Metadata>, IEnumerable
{
    public readonly Module Module;
    public Guid ID => Module.ModuleVersionId;
    readonly ImmutableDictionary<Guid, ImmutableArray<Metadata>> Data;
    MetadataMap(Dictionary<Guid, ImmutableArray<Metadata>> data)
    {
        Module = data.First().Value.First().Module;
        Data = data.ToImmutableDictionary();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var pair in Data)
        {
            foreach (var element in pair.Value)
            {
                yield return element;
            }
        }
    }

    public IEnumerator<Metadata> GetEnumerator()
    {
        return GetEnumerator();
    }


    public ImmutableArray<Metadata> this[Type index]
    {
        get => Data[index.GUID];
    }
    /// <summary>
    /// Maps all the metadata in a module to a dictionary, keyed by type with a list of all the type's members in metadata format.
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public static MetadataMap New(Module m)
    {
        Dictionary<Guid, ImmutableArray<Metadata>> metadata = [];
        ResolveAllMetadata(metadata, m.GetTypes());
        return new MetadataMap(metadata);

    }
    static void ResolveAllMetadata(Dictionary<Guid, ImmutableArray<Metadata>> metadata, Type[] types)
    {
        const BindingFlags allDeclared = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        foreach (var type in types)
        {
            List<Metadata> typedata = [];
            typedata.Add(new(type));
            GetMetadataOf(typedata, type.GetProperties(allDeclared));
            GetMetadataOf(typedata, type.GetFields(allDeclared));
            GetMetadataOf(typedata, type.GetConstructors(allDeclared));
            GetMetadataOf(typedata, type.GetMethods(allDeclared));
            GetMetadataOf(typedata, type.GetEvents(allDeclared));
            //  GetMetadataOf(metadata, type.GetMembers(Default));
            metadata[type.GUID] = [.. typedata];
        }
    }

    static void GetMetadataOf<T>(List<Metadata> metadata, T[] array) where T : MemberInfo
    {
        foreach (T obj in array)
        {
            metadata.Add(new(obj));
        }
    }



}