using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace BonesClassLibrary.Reflection;

/// <summary>
/// Readable IL instruction from a byte array, used by ILParser.
/// </summary>
public sealed class ILInstruction
{
    readonly int Offset;
    public readonly OpCode OpCode;
    public readonly object? Operand;
    private ILInstruction()
    {
        
    }
    internal ILInstruction(OpCode opcode, object? operand, int offset)
    {
        OpCode = opcode;
        Operand = operand;
        Offset = offset;
    }
    public override string ToString()
    {
        return $"IL_{Offset.ToString("x4")}: {OpCode} {OperandToString()}";
    }
    string OperandToString()
    {
        if (Operand == null)
            return "";
        else if (OpCode.OperandType == OperandType.InlineBrTarget || OpCode.OperandType == OperandType.ShortInlineBrTarget)
        {
            long num = (long)Operand;
            return $"IL_{num.ToString("x4")}";
        }
        else if (Operand is string)
            return $"\"{Operand}\"";
        else if (Operand is MethodInfo info)
        {
            string sttc = info.IsStatic ? "static" : "instance";
            string ret = string.IsNullOrWhiteSpace(info.ReturnParameter.Name) ? "void" : info.ReturnParameter.Name.ToLower();
            return $"{sttc} {ret} {info.DeclaringType}::{info.Name}{ParamsToString(info.GetParameters())}";
        }
        else if (Operand is ConstructorInfo cstr)
        {
            return $"{cstr.DeclaringType}::.ctor{ParamsToString(cstr.GetParameters())}";
        }
        else if (Operand.ToString() is string strng)
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