using System.Reflection;
using System.Text;
using HarmonyLib;
using Mono.Reflection;
using BonesClassLibrary.FileFinders;

namespace BonesClassLibrary.Reflection;

public abstract class ILReader
{
    protected static string OperandToString(object? obj)
    {
        if (obj == null)
            return "";
        else if (obj is string)
        return $"\"{obj}\"";
        else if (obj is MethodInfo info)
        {
            string sttc = info.IsStatic ? "static" : "instance";
            string ret = string.IsNullOrWhiteSpace(info.ReturnParameter.Name) ? "void" : info.ReturnParameter.Name.ToLower();
            return $"{sttc} {ret} {info.DeclaringType}::{info.Name}{ParamsToString(info.GetParameters())}";
        }
        else if (obj is ConstructorInfo cstr)
        {
            return $"{cstr.DeclaringType}::.ctor{ParamsToString(cstr.GetParameters())}";
        }
        else if (obj.ToString() is string strng)
            return strng;
        return "";
    }

    static string ParamsToString(ParameterInfo[] args)
    {
        StringBuilder txt = new();
        txt.Append('(');
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            txt.Append($"{arg.ParameterType.Name.ToLower()}");
            if (args.Length > 1 && i < args.Length - 1)
                txt.Append($", ");
        }
        txt.Append(')');
        return txt.ToString();
    }
}