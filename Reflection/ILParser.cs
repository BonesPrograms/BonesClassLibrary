using System.Reflection.Emit;
using System.Reflection;
using BonesClassLibrary.FileFinders;

namespace BonesClassLibrary.Reflection;


/// <summary>
/// This is my own foray into parsing IL. It is less informative than ILReader, it is for learning.
/// </summary>

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
public sealed class ILParser
{
    /// <summary>
    /// Converts a method into readable IL instructions.
    /// </summary>
    /// 
    readonly byte[] IL;
    readonly IList<LocalVariableInfo> Locals;
    readonly Module Module;
    readonly ParameterInfo[] Params;
    readonly MethodInfo Method;
    const byte ZeroBits = 0x00;
    const byte EightBits = 0x01;
    const byte SixteenBits = 0x02;
    const byte ThirtyTwoBits = 0x04;
    const byte SixtyFourBits = 0x08;
    const byte PrefixBit = 0xFE;
    List<ILInstruction> Instructions = null!;
    int I = 0;
    int Offset = 0;
    public ILParser(MethodInfo method)
    {
        MethodBody? body = method.GetMethodBody();
        byte[]? il = body?.GetILAsByteArray();
        ThrowIfNull(body, il, method);
        Locals = body!.LocalVariables;
        IL = il!;
        Module = method.Module;
        Params = method.GetParameters();
        Method = method;
    }

    static void ThrowIfNull(MethodBody? body, byte[]? il, MethodInfo method)
    {
        if (body == null || il == null)
        {
            string msg = string.Empty;
            if (method.GetMethodBody() == null)
                msg += $"Could not get method body from {method} :: ";
            if (il == null)
                msg += $"Could not get IL bytes from {method}";
            throw new InvalidOperationException(msg);
        }
    }

    public void PrintIL(string? path = null)
    {
        if (Instructions == null)
            GetIL();
        path ??= Path.Combine(VietnamWarModLab.Path, @"BonesClassLibrary\Reflection\methodread.il");
        List<string> text = new(Instructions!.Count);
        foreach (ILInstruction instruction in Instructions)
        {
            text.Add(instruction.ToString());
        }
        StreamWriter writer = new(path);
        foreach (string txt in text)
        {
            writer.WriteLine(txt);
        }
        writer.Close();
    }
    public List<ILInstruction> GetIL()
    {
        if (Instructions == null)
        {
            Instructions = [];
            while (I < IL.Length)
                BytesToIL();
        }
        return Instructions;
    }

    void BytesToIL()
    {
        OpCode code = GetOpCode();
        int size = OperandSize(code.OperandType);
        long token = GetToken(ref size, code.OperandType);
        object? operand = size == ZeroBits ? null : GetOperand(code, token);
        Instructions.Add(new(code, operand, Offset));
        I += size;
        Offset = I;
    }

    OpCode GetOpCode()
    {
        OpCode code;
        byte hex = IL[I];
        if (hex == PrefixBit)
        {
            byte nextbyte = IL[I + 1];
            short key = (short)((PrefixBit << 0x08) | nextbyte); //learn how to do bitwise!
            code = OpCodeMap.OpCodes[key];
            I += 2;
        }
        else
        {
            code = OpCodeMap.OpCodes[hex];
            I++;
        }
        return code;
    }

    long GetToken(ref int size, OperandType? type = null)
    {
        long token = 0;
        if (size == ZeroBits)
            return token;
        else if (size == EightBits)
            token = IL[I];
        else if (size == SixteenBits)
            token = BitConverter.ToInt16(IL, I);
        else if (size == ThirtyTwoBits)
        {
            token = BitConverter.ToInt32(IL, I);
            if (type == OperandType.InlineSwitch)
                size += (int)token * 4;
        }
        else if (size == SixtyFourBits)
            token = BitConverter.ToInt64(IL, I); //64 bytes
        return token;
    }

    object? GetOperand(OpCode code, long token)
    =>
        code.OperandType switch
        {
            OperandType.InlineI or OperandType.ShortInlineI or OperandType.InlineR or OperandType.ShortInlineR or OperandType.InlineI8 => token,
            OperandType.InlineMethod => Module.ResolveMethod((int)token),
            OperandType.InlineField => Module.ResolveField((int)token),
            OperandType.InlineType => Module.ResolveType((int)token),
            OperandType.InlineString => Module.ResolveString((int)token),
            OperandType.InlineTok => Module.ResolveMember((int)token),
            OperandType.ShortInlineVar => GetVariable(code, (int)token),
            OperandType.InlineVar => GetVariable(code, (int)token),
            OperandType.InlineSig => Module.ResolveSignature((int)token),
            OperandType.InlineNone or OperandType.InlineSwitch => null,
            OperandType.InlineBrTarget or OperandType.ShortInlineBrTarget => token + I + 1, //idk why u need to do +1 but it is always -1 instruction off from its actual jump target
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
            else if (token == 0)
                return "this"; //idk how to get this, was trying to research it monocecil, but cant access the class they use to resolve waht this is
            return Params[token - 1];
        }
    }

#pragma warning disable CS8509 // OperandType.InlinePhi is excluded because it is obsolete.
    static byte OperandSize(OperandType type) =>
    type switch
    {
        OperandType.InlineNone => ZeroBits,
        OperandType.ShortInlineVar or OperandType.ShortInlineI or OperandType.ShortInlineBrTarget => EightBits,
        OperandType.InlineVar => SixteenBits,
        OperandType.InlineTok or OperandType.InlineBrTarget or OperandType.InlineField or OperandType.InlineI or OperandType.InlineMethod or OperandType.InlineSig or OperandType.InlineString or OperandType.InlineType or OperandType.ShortInlineR or OperandType.InlineSwitch => ThirtyTwoBits,
        OperandType.InlineR or OperandType.InlineI8 or OperandType.InlineR => SixtyFourBits
    };
#pragma warning restore CS8509
}
