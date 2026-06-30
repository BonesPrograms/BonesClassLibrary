using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Collections.Immutable;
using System.Numerics;
using BonesClassLibrary.Extensions;
using static BonesClassLibrary.Bytes.ByteReader;

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
    public readonly IReadOnlyList<byte>? Bytes;
    public readonly object? Token;
    readonly byte[]? _bytes;
    internal ByteCode(OpCode opcode, object? operand, int offset, object token) : base(operand)
    {
        OpCode = opcode;
        Offset = offset;
        if (Operand is not LocalVariableInfo && Operand is not ParameterInfo && TokenDoesntEqualOperand(operand, token))
        {
            Token = token;
            _bytes = [.. TokenToBytes(token)];
            Bytes = _bytes.AsReadOnly();
        }
    }

    static byte[] TokenToBytes(object token)
    {
        if (token is byte bits && bits == x0bit)
            return [];
        switch (token)
        {
            case byte x8bits:
                {
                    byte[] bytes = [x8bits];
                    return bytes;
                }
            case short int16:
                return int16.AsBytes();
            case int int32:
                return int32.AsBytes();
            case float float32:
                return float32.AsBytes();
            case long int64:
                return int64.AsBytes();
            case double float64:
                return float64.AsBytes();
        }
        return [];
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
        sb.Append(" || ");
        sb.Append(" OpCode Bytes: ");
        sb.Append(BytesToString(OpCode.Value.AsBytes()));
        if (Bytes != null)
        {
            sb.Append($":: Token: {Token} ");
            sb.Append($"AsBytes: ");
            sb.Append(BytesToString(_bytes!));
        }
        return sb;
    }

    static bool TokenDoesntEqualOperand(object? operand, object token)
    {
        if (operand == null)
            return false;
        if (operand is short shrt && token is short sht)
            return shrt != sht;
        else if (operand is int intgr && token is int tkn)
            return intgr != tkn;
        else if (operand is byte bits && token is byte bitz)
            return bits != bitz;
        else if (operand is long lng && token is long lngr)
            return lng != lngr;
        return true;
    }

    static StringBuilder BytesToString(byte[] bytes)
    {
        StringBuilder sb = new();
        for (int i = 0; i < bytes.Length; i++)
            sb.Append($"{bytes[i]} ");
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