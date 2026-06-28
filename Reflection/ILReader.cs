using System.Reflection.Emit;
using System.Reflection;
using BonesClassLibrary.FileFinders;
using System.Buffers.Binary;

namespace BonesClassLibrary.Reflection;



// You use the OperandType to determine how many bytes to read to get the meaningful data (the token or offset).
// Identify the Opcode (e.g., ldfld, call).
// Check OperandType to see if it needs 1, 2, 4, or 8 bytes of data.
// Read those bytes using BitConverter.
// Use the result:
// For call/ldfld: The result is a Metadata Token (int). You pass this to Module.ResolveMethod(token) or Module.ResolveField(token) to get the actual MethodInfo or FieldInfo.
// For brfalse: The result is a Relative Offset. You add it to the current position to find the target label.

// Yes, you are missing several common operand types. Your current code will return null for jumps, variables, arguments, and floating-point numbers, causing you to lose that data. 

// Here are the critical cases you need to add:

// Branches (Jumps): InlineBrTarget (4 bytes) and ShortInlineBrTarget (1 byte). These are crucial for if, while, and foreach logic.
// Variables & Arguments: InlineVar (2 bytes) and ShortInlineVar (1 byte) for locals (ldloc), plus InlineArg and ShortInlineArg for arguments (ldarg).
// Floating Point: InlineR (8 bytes) and ShortInlineR (4 bytes) for double and float constants.
// Long Integers: InlineI8 (8 bytes) for long constants.
// Switch: InlineSwitch (variable length) for switch statements.

//See Mono.Reflection.MethodBodyReader to learn more about reading method bodies! and Mono.Reflection.Instruction for instruction learning

//learn wtf an offset is in IL

//Update:
//This has gotten pretty good now, the only thing it cant really read yet are jump labels. Otherwise it is able to read shit pretty much just as good as the MonoCecil ILReader

/// <summary>
/// Converts a method into readable IL instructions. 
/// </summary>
public sealed class ILReader
{
    public readonly IReadOnlyList<byte> RawBytes;
    readonly IList<LocalVariableInfo> Locals;
    readonly ParameterInfo[] Params;
    public readonly Module Module;
    public readonly MethodInfo Method;

    /// <summary>
    /// 0 bytes.
    /// </summary>
    const byte ZeroBit = 0x00; //0 bytes

    /// <summary>
    /// 1 byte.
    /// </summary>
    const byte EightBit = 0x01; 
    /// <summary>
    /// 2 bytes.
    /// </summary>
    const byte SixteenBit = 0x02; 
    /// <summary>
    /// 4 bytes.
    /// </summary>
    const byte ThirtyTwoBit = 0x04; 
    /// <summary>
    /// 8 bytes.
    /// </summary>
    const byte SixtyFourBit = 0x08;

    /// <summary>
    /// Some opcodes are 2 bytes long, they will always start with a "prefix" byte.
    /// </summary>
    const byte PrefixBit = 0xFE; 
    readonly byte[] IL;
    readonly Type[]? GenericMethodArgs;
    readonly Type[]? GenericTypeArgs;
    public ILReader(MethodInfo method)
    {
        MethodBody body = method.GetMethodBody() ?? throw new ArgumentException("Method body is null.");
        IL = body?.GetILAsByteArray() ?? throw new ArgumentException("Byte array is null.");
        RawBytes = IL.AsReadOnly();
        Locals = body.LocalVariables;
        Params = method.GetParameters();
        Module = method.Module;
        Method = method;
        GenericMethodArgs = method.GetGenericArguments();
        GenericTypeArgs = method.DeclaringType?.GetGenericArguments();
    }

    /// <summary>
    /// Prints readable IL to a file.
    /// </summary>
    /// <param name="path"></param>
    public void PrintIL(string? path = null) //sho
    {
        var codes = GetIL();
        path ??= Path.Combine(VietnamWarModLab.Path, @"BonesClassLibrary\Reflection\methodread.il");
        StreamWriter writer = new(path);
        foreach (ByteCode code in codes)
        {
            writer.WriteLine(code.ToString());
        }
        writer.Close();
    }

    /// <summary>
    /// Returns a list of readable IL.
    /// </summary>
    /// <returns></returns>
    public List<ByteCode> GetIL()
    {
        int i = 0;
        List<ByteCode> codes = [];
        while (i < IL.Length)
            BytesToIL(codes, ref i);
        return codes;
    }

    void BytesToIL(List<ByteCode> codes, ref int i)
    {
        OpCode code = GetOpCode(ref i);
        int size = OperandSize(code.OperandType);
        var token = GetToken(i, ref size, code.OperandType);
        object? operand = size == ZeroBit ? null : GetOperand(code, token, i);
        codes.Add(new(code, operand, i - 1)); //for some reason it is off by 1, you want to shift the offset back by 1 or every label will be off by 1 individually and cumulatively so all labels will be off
        i += size;
    }

    OpCode GetOpCode(ref int i)
    {
        OpCode code;
        byte indexedbyte = IL[i];
        if (indexedbyte == PrefixBit)
        {
            byte nextbyte = IL[i + 1];
            short key = (short)((PrefixBit << 0x08) | nextbyte); //learn how to do bitwise!
            code = OpCodeMap.OpCodes[key];
            i += 2;
        }
        else
        {
            code = OpCodeMap.OpCodes[indexedbyte];
            i++;
        }
        return code;
    }

    //This needs to return an object for the token, because the token is sometimes just a numeric value for a struct and not an actual 32bit metadata token

    //First off, if you have a double or a float, you will lose floating point precision if you do not return it as anything less than a double.
    //Second off, if you have a long, you will lose integer precision if you return it as a double

    //A decimal is 128 bits, but still cannot fully maintain integer precision, floating point precision, range and magnitude
    //of other integral numeric types (such as a double or float) that are cast to decimal.

    //I should note - actual Int128 structs will be readable until they reach around 19-20 digits (a long can only hold 19 digits)
    //So far, this cannot be remedied - there is no 128 bit operand type, and even real decompilers (like ilspy)
    //will not properly decompile the UInt128 number 11111111111111111111, for example. Becaue ilspy doesnt do it, i wont do it either, lol!
    object GetToken(int i, ref int size, OperandType type)
    {
        if (size == ZeroBit)
            return ZeroBit; //no operand
        else if (size == EightBit)
            return (int)IL[i]; //if you dont cast this, it will throw later when you pass it as a parameter to GetVariable, claiming it cannot cast system.byte to system.int32
        else if (size == SixteenBit)
            return BinaryPrimitives.ReadInt16LittleEndian(IL.AsSpan(i, SixteenBit)); //BitConverter works, but BinaryPrimitives will automatically handle it without us
        else if (size == ThirtyTwoBit)                                       //having to reverse the bytes to get their token on BigEndian systems
        {                                                                       //IL is always stored as little endian, but at runtime could be bigendian
            int token;
            if (type == OperandType.ShortInlineR)
                return BinaryPrimitives.ReadSingleLittleEndian(IL.AsSpan(i, ThirtyTwoBit));
            else
                token = BinaryPrimitives.ReadInt32LittleEndian(IL.AsSpan(i, ThirtyTwoBit)); ;
            if (type == OperandType.InlineSwitch)
                size += token * 4;
            return token;
        }
        else if (size == SixtyFourBit)
        {
            if (type == OperandType.InlineR)
                return  BinaryPrimitives.ReadDoubleLittleEndian(IL.AsSpan(i, SixtyFourBit));
            return BinaryPrimitives.ReadInt64LittleEndian(IL.AsSpan(i, SixtyFourBit));
        }
        throw new NotSupportedException("Instruction is not 0-bit, 8-bit, 16-bit, 32-bit, or 64-bit!!!! How did this happen?!");
    }

    object? GetOperand(OpCode code, object token, int i)
    =>
        code.OperandType switch
        {
            OperandType.ShortInlineI => (int)token,
            OperandType.InlineI => (int)token,
            OperandType.InlineR => (double)token,
            OperandType.ShortInlineR => (float)token,
            OperandType.InlineI8 => (long)token,
            OperandType.InlineMethod => Module.ResolveMethod((int)token, GenericTypeArgs, GenericMethodArgs),
            OperandType.InlineField => Module.ResolveField((int)token, GenericTypeArgs, GenericMethodArgs),
            OperandType.InlineType => Module.ResolveType((int)token, GenericTypeArgs, GenericMethodArgs),
            OperandType.InlineString => Module.ResolveString((int)token),
            OperandType.InlineTok => Module.ResolveMember((int)token, GenericTypeArgs, GenericMethodArgs),
            OperandType.ShortInlineVar => GetVariable(code, (int)token),
            OperandType.InlineVar => GetVariable(code, (int)token),
            OperandType.InlineSig => Module.ResolveSignature((int)token),
            OperandType.InlineNone or OperandType.InlineSwitch => null,
            OperandType.InlineBrTarget or OperandType.ShortInlineBrTarget => (int)token + i + 1, //idk why u need to do +1 but it is always -1 instruction off from its actual jump target
            _ => throw new NotSupportedException()
        };

    object GetVariable(OpCode opcode, int token)
    {
        if (opcode.Name?.Contains("loc") ?? false)
        {
            return Locals[token];
        }
        else
        {
            if (Method.IsStatic)
            {
                return Params[token];
            }
            else if (token == 0) //idk why a token of 0 == this, but i guess that is the byte code for this, wonder where i can learn that
                return "this"; //idk how to get this, was trying to research it monocecil, but cant access the class they use to resolve waht this is
            return Params[token - 1]; //not sure why they do -1 here, not sure why i do +1 for the jump label lol
        }                              //i suppose it has something to do with being nonstatic? but why -1? is "this" on the params list?
    }

#pragma warning disable CS8509 // OperandType.InlinePhi is excluded because it is obsolete.
    static byte OperandSize(OperandType type) =>
    type switch
    {
        OperandType.InlineNone => ZeroBit,
        OperandType.ShortInlineVar or OperandType.ShortInlineI or OperandType.ShortInlineBrTarget => EightBit,
        OperandType.InlineVar => SixteenBit,
        OperandType.InlineTok or OperandType.InlineBrTarget or OperandType.InlineField or OperandType.InlineI or OperandType.InlineMethod or OperandType.InlineSig or OperandType.InlineString or OperandType.InlineType or OperandType.ShortInlineR or OperandType.InlineSwitch => ThirtyTwoBit,
        OperandType.InlineR or OperandType.InlineI8 or OperandType.InlineR => SixtyFourBit
    };
#pragma warning restore CS8509
}
