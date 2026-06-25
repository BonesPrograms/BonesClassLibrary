using System.Reflection;
using System.Text;
using HarmonyLib;
using Mono.Reflection;
using System.Reflection.Emit;
using BonesClassLibrary.FileFinders;

namespace BonesClassLibrary.Reflection;


/// <summary>
/// This class uses MonoCecil to convert a method to readable IL and prints it to a file.
/// </summary>
public static class ILReader
{

    public static void PrintIL(Type type, string name, string? path = null)
    {
        PrintIL(AccessTools.Method(type, name), path);
    }
    public static void PrintIL(MethodInfo method, string? path = null)
    {
        path ??= Path.Combine(VietnamWarModLab.Path, @"BonesClassLibrary\Reflection\methodread.il");
        Instruction[] codes = [.. method.GetInstructions()];
        List<string> text = new(codes.Length);
        foreach (var code in codes)
        {
            string txt = MethodToString(code.Operand);
            text.Add($" {LabelSnip(code.ToString())}{code.OpCode} {txt}");
        }
        StreamWriter writer = new(path);
        foreach (var txt in text)
        {
            writer.WriteLine(txt);
        }
        writer.Close();
    }

    static string LabelSnip(string txt)
    {
        StringBuilder label = new();
        for (int i = 0; i < 7; i++) //idk why i put i < 7 here lol? but it works for now so well keep it. oh i see - 7 is the max character length for a jump label
        {
            label.Append(txt[i]);
        }
        label.Append(": ");
        return label.ToString();
    }

    static string MethodToString(object? obj)
    {
        if (obj is MethodInfo info)
        {
            string sttc = info.IsStatic ? "static" : "instance";
            string ret = string.IsNullOrWhiteSpace(info.ReturnParameter.Name) ? "void" : info.ReturnParameter.Name.ToLower();
            return $"{sttc} {ret} {info.DeclaringType}::{info.Name}{ParamsToString(info.GetParameters())}";
        }
        else if (obj is ConstructorInfo cstr)
        {
            return $"{cstr.DeclaringType}{ParamsToString(cstr.GetParameters())}";
        }
        return obj == null ? "" : obj.ToString()!;
    }

    static string ParamsToString(ParameterInfo[] args)
    {
        StringBuilder txt = new();
        txt.Append('(');
        foreach (var arg in args)
        {
            txt.Append($"{arg.ParameterType.Name.ToLower()}");
        }
        txt.Append(')');
        return txt.ToString();
    }
}