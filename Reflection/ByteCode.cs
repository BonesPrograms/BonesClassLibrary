using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Collections.Immutable;

namespace BonesClassLibrary.Reflection;

public static class OpCodeMap
{

    /// <summary>
    /// A dictionary of opcodes that can be indexed by their short value (OpCode.Value).
    /// </summary>
    public static readonly ImmutableDictionary<short, OpCode> OpCodes =
    typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public)
    .Where(x => x.FieldType == typeof(OpCode))
    .Select(x => (OpCode)x.GetValue(null)!)
    .ToImmutableDictionary(k => k.Value, v => v);
}
/// <summary>
/// Readable IL instruction.
/// </summary>
public sealed class ByteCode : MetadataReader
{
    public readonly int Offset;
    public readonly OpCode OpCode;
    public object? Operand => Object;
    internal ByteCode(OpCode opcode, object? operand, int offset) : base(operand)
    {
        OpCode = opcode;
        Offset = offset;
    }
    protected override StringBuilder ToStringBuilder()
    {
        StringBuilder sb = new();
        sb.Append($"IL_{Offset:x4}: ");
        sb.Append(OpCode.ToString());
        if (Object is ConstructorInfo)
            sb.Append(" instance void");
        sb.Append(' ');
        sb.Append(OperandToString());
        return sb;
    }
    StringBuilder OperandToString()
    {
        
        if (OpCode.OperandType == OperandType.InlineBrTarget || OpCode.OperandType == OperandType.ShortInlineBrTarget)
        {
            int num = (int)Operand!; //jump target offsets are max 32bit and will never be null
            return new StringBuilder($"IL_{num:x4}");
        }
        else if (Object is string)
            return new StringBuilder($"\"{Object}\"");
        return base.ToStringBuilder();

    }
}