using System.Reflection;
using BonesClassLibrary.Reflection;

namespace BonesClassLibrary.Extensions;

public static class ReflectionExtensions
{
    public static Metadata AsMetadata<T>(this T obj) where T : class
    {
        MemberInfo info = obj is MemberInfo inf ? inf : obj.GetType();
        return new(info);
    }
}