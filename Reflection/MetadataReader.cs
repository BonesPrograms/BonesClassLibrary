using System.Reflection;
using System.Text;
using HarmonyLib;

namespace BonesClassLibrary.Reflection;

/// <summary>
/// Base wrapper class with robust ToString helpers for metadata objects.
/// </summary>
public abstract class MetadataReader
{

    //I may add an option later for displaying namespaces on type names. For now we do not to improve readability.
    protected readonly object? Object;
    public MetadataReader(object? obj)
    {
        Object = obj;
    }

    /// <summary>
    /// Virtual tostring stringbuilder for inheritors.
    /// </summary>
    /// <returns></returns>
    protected virtual StringBuilder ToStringBuilder() => Object switch
    {
        MethodInfo => MethodToString((MethodInfo)Object),
        ConstructorInfo => ConstructorToString((ConstructorInfo)Object),
        FieldInfo or PropertyInfo or EventInfo => MemberToString((MemberInfo)Object),
        Type => TypeToString((Type)Object),
        _ => new StringBuilder(Object?.ToString() ?? "") 
    };                                                   
                                                   
                                                  
    public override sealed string ToString() //inheritors should not invoke tostring in their tostringbuilder override otherwise it will obviously create duplicate stringbuilders
    {
        return ToStringBuilder().ToString();
    }
    static StringBuilder MemberToString(MemberInfo member)
    {
        StringBuilder sb = new();
        sb.Append(member.MemberType.ToString().ToLower());
        sb.Append(' ');
        sb.Append(CheckTypeGenerics(member.DeclaringType));
        sb.Append("::");
        sb.Append(CheckTypeGenerics(member.GetUnderlyingType()));
        sb.Append(' ');
        sb.Append(FixGenericString(member.Name));
        return sb;
    }

    static StringBuilder TypeToString(Type type)
    {
        StringBuilder sb = new();
        if (type.IsEnum)
            sb.Append("enum");
        else if (type.IsArray)
            sb.Append("array");
        else if (type.IsInterface)
            sb.Append("interface");
        else if (type != typeof(string) && (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) || typeof(System.Collections.ICollection).IsAssignableFrom(type)))
            sb.Append("collection");
        else if (type.IsClass)
            sb.Append("class");
        else
            sb.Append("struct");
        sb.Append(' ');
        sb.Append(CheckTypeGenerics(type));
        return sb;
    }
    static StringBuilder ConstructorToString(ConstructorInfo ctor)
    {
        StringBuilder sb = new();
        if (ctor.DeclaringType != null)
        {
            sb.Append(CheckTypeGenerics(ctor.DeclaringType));
        }
        sb.Append($"::.ctor{ParamsToString(ctor.GetParameters())}");
        return sb;
    }

    static StringBuilder MethodToString(MethodInfo mthd)
    {
        StringBuilder sb = new();
        sb.Append(mthd.IsStatic ? "static " : "instance ");

        string ret = string.IsNullOrWhiteSpace(mthd.ReturnType.Name) ? "void" : mthd.ReturnType.Name.ToLower();
        if (ret == "boolean")
            ret = "bool";
        sb.Append(FixGenericString(ret));
        sb.Append(' ');

        sb.Append(CheckTypeGenerics(mthd.DeclaringType));
        sb.Append("::");
        sb.Append(mthd.Name);

        AddGenericArguments(sb, mthd.GetGenericArguments());
        sb.Append(ParamsToString(mthd.GetParameters()));

        return sb;

    }

    static StringBuilder ParamsToString(ParameterInfo[] args)
    {
        StringBuilder txt = new();
        txt.Append('(');
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            txt.Append(CheckTypeGenerics(arg.ParameterType));
            if (args.Length > 1 && i < args.Length - 1)
                txt.Append($", ");
        }
        txt.Append(')');
        return txt;
    }
    /// <summary>
    /// Be careful using this and FixGenericString together or you will not understand why you are producing duplicate name strings.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static StringBuilder? CheckTypeGenerics(Type? type)
    {
        if (type != null)
        {
            Type[] generics = type.GetGenericArguments();
            StringBuilder sb = new();
            sb.Append(FixGenericString(type.Name)); //adds name string here
            AddGenericArguments(sb, generics);
            return sb;
        }
        return null;
    }

    static string FixGenericString(string strng)
    {
        if (strng.Length >= 2 && strng[^2] == '`')
            strng = strng[..strng.IndexOf('`')];
        return strng;
    }

    static void AddGenericArguments(StringBuilder sb, Type[]? genericargs)
    {
        if (genericargs?.Length > 0)
        {
            sb.Append('<');
            for (int i = 0; i < genericargs.Length; i++)
            {
                sb.Append(FixGenericString(genericargs[i].Name));
                if (genericargs.Length > 1 && i < genericargs.Length - 1)
                    sb.Append(", ");
            }
            sb.Append('>');
        }
    }

}