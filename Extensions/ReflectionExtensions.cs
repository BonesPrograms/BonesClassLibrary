using System.Reflection;
using BonesClassLibrary.Reflection;

namespace BonesClassLibrary.Extensions;

public static class ReflectionExtensions
{
    public static Metadata AsMetadata(this MemberInfo info) => Metadata.Get(info);
}