using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Collections.Immutable;
using System.Numerics;
using BonesClassLibrary.Extensions;
using static BonesClassLibrary.Bytes.ByteReader;
using System.ComponentModel;

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
#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static bool ShowOpCodeBytes = true;
    public static bool ShowOperandBytes = true;

#pragma warning restore CA2211 // Non-constant fields should not be visible
    public readonly int Offset;
    public readonly OpCode OpCode;
    public object? Operand => Object;
    public readonly int? Token; //MetadataToken
    public readonly IReadOnlyList<byte>? OperandBytes;
    readonly byte[]? _bytes;
    public readonly bool HasOperand; //this does not mean that OperandBytes is not null, it just means this opcode has an operand - if bytes are null, this implies a single byte operand
    internal ByteCode(OpCode opcode, object? operand, int offset, object token) : base(operand)
    {
        OpCode = opcode;
        Offset = offset;
        if (operand is not null and not LocalVariableInfo and not ParameterInfo)
        {
            HasOperand = true;
            if (TokenDoesntEqualOperand(operand, token)) //if token and operand are equal this means you are receiving a raw numeric value
                Token = (int)token;  //which is not a metadata token
            byte[]? bytes = TokenToBytes(token);  //however it still has operand bytes so we retrieve the operand bytes
            if (bytes != null && BytesDontEqualOperand(bytes, operand)) //but id rather not display the bytes if the operand's value can be represented in a single byte
            {
                _bytes = [.. bytes];
                OperandBytes = _bytes.AsReadOnly();
            }
        }
    }

    static byte[]? TokenToBytes(object token) => token switch
    {
        short int16 => int16.AsBytes(),
        int int32 => int32.AsBytes(),
        float float32 => float32.AsBytes(),
        long int64 => int64.AsBytes(),
        double float64 => float64.AsBytes(),
        _ => null
    };
    
    protected override StringBuilder ToStringBuilder()
    {
        StringBuilder sb = new();
        sb.Append($"IL_{Offset:x4}: ");
        sb.Append(OpCode.ToString());
        if (Object is ConstructorInfo)
            sb.Append(" instance void");
        sb.Append(' ');
        sb.Append(OperandToString());
        if (ShowOpCodeBytes || ShowOperandBytes)
            sb.Append(" || ");
        if (ShowOpCodeBytes)
        {
            sb.Append(" OpCodeBytes: ");
            sb.Append(BytesToString(OpCode.Value.AsBytes()));
        }
        if (OperandBytes != null && ShowOperandBytes)
        {
            if (Token != null)
                sb.Append($" :: Token: {Token} ");
            sb.Append(' ');
            sb.Append(Token == null ? "Operand" : "Token");
            sb.Append("Bytes: ");
            sb.Append(BytesToString(_bytes!));
        }
        return sb;
    }
    static bool BytesDontEqualOperand(byte[]? bytes, object operand)
    {
        if (bytes?.Length == 1)
        {
            if (operand is byte bits)
            {
                if (bytes[0] == bits)
                    return false;
            }
            else if (operand is int intgr) //check for short too? no i think this only needs to be byte based - this is for chars?
            {
                if (bytes[0] == intgr)
                    return false;
            }
        }
        return true;
    }
    static bool TokenDoesntEqualOperand(object? operand, object token)
    {
        if (operand == null)
            return true;
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