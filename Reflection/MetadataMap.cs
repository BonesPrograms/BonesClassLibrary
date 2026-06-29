using System.Reflection;
using BonesClassLibrary.Reflection;
using System.Collections.Immutable;

namespace Bones.Collections;


///Mission plan:
/// Metadata.MetadataMap returns a Dictionary with a Type Key that has a List Collection
/// Collections inside of collections are annoying
/// Were going to make it require one less foreach look to be able to reach the value by making our own better enumerator
/// 
/// 

///Map format: declared only
/// 




/// <summary>
/// Dictionary wrapper that supports indexing by metadata or by memberinfo. Simplifies enumeration by dumping out Type objects and then following that with all of the type's
/// members before moving on to the next type object.
/// </summary>
public sealed class MetadataMap : IEnumerable<Metadata>
{
    public readonly ImmutableDictionary<MemberInfo, ImmutableList<Metadata>> MemberInfoDictionary;
    public readonly ImmutableDictionary<Metadata, ImmutableList<Metadata>> MetadataDictionary;
    public MetadataMap(Dictionary<Metadata, ImmutableList<Metadata>> data)
    {
        MemberInfoDictionary = data.ToImmutableDictionary(x => x.Key.Info, y => y.Value);
        MetadataDictionary = data.ToImmutableDictionary();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        foreach (var pair in MetadataDictionary)
        {
            yield return pair.Key;
            foreach (var element in pair.Value)
            {
                yield return element;
            }
        }
    }

    public IEnumerator<Metadata> GetEnumerator()
    {
        foreach (var pair in MetadataDictionary)
        {
            yield return pair.Key;
            foreach (var element in pair.Value)
            {
                yield return element;
            }
        }
    }


    public ImmutableList<Metadata> this[MemberInfo index]
    {
        get => MemberInfoDictionary[index];
    }

    public ImmutableList<Metadata> this[Metadata index]
    {
        get => MetadataDictionary[index];
    }



}