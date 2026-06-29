
namespace BonesClassLibrary.Extensions;

public static class CollectionExtensions
{
    public static void ForEach<T>(this IEnumerable<T> objs, Action<T> action)
    {
        foreach (var obj in objs)
            action(obj);
    }
}