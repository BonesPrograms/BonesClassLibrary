using System.Reflection.Emit;
using System.Reflection;

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

//See Mono.Reflection.MethodBodyReader to learn more about reading method bodies!

//Update:
//This has gotten pretty good now, the only thing it cant really read yet are jump labels. Otherwise it is able to read shit pretty much just as good as the MonoCecil ILReader
public static class ILParser
{
    public static List<ILInstruction> GetIL(MethodInfo method)
    {
        MethodBody? body = method.GetMethodBody();
        byte[]? il = body?.GetILAsByteArray();
        if (il != null)
            return BytesToIL(il, method.Module);
        else
        {
            string msg = string.Empty;
            if (body == null)
                msg += $"Could not get method body from {method} :: ";
            if (il == null)
                msg += $"Could not get IL bytes from {method}";
            throw new InvalidOperationException(msg);
        }
    }
    static List<ILInstruction> BytesToIL(byte[] il, Module module)
    {
        List<ILInstruction> codes = [];
        int i = 0;
        while (i < il.Length)
        {
            byte bit = il[i];
            OpCode code = GetOpCode(il, bit, ref i);
            int size = OperandSize(code.OperandType);
            long token = GetToken(il, i, ref size, code.OperandType);
            object? operand = size == 0x00 ? null : GetOperand(code.OperandType, il, i, (int)token, module);
                        i += size;
            codes.Add(new(code, operand));
        }
        return codes;
    }

    static OpCode GetOpCode(byte[] il, byte bit, ref int i)
    {
        OpCode code;
        if (bit == 0xFE)
        {
            byte nextbyte = il[i + 1];
            short key = (short)((0xFE << 8) | nextbyte);
            code = OpCodeMap.OpCodes[key];
            i += 2;
        }
        else
        {
            code = OpCodeMap.OpCodes[bit];
            i++;
        }
        return code;
    }

    static long GetToken(byte[] il, int i, ref int size, OperandType type)
    {
        long token = default;
        if (size == 0x01)
            token = il[i];
        else if (size == 0x02)
            token = BitConverter.ToInt16(il, i);
        else if (size == 0x04)
        {
            token = BitConverter.ToInt32(il, i);
            if (type == OperandType.InlineSwitch)
                size += (int)token * 4;
        }
        else if (size == 0x08)
            token = BitConverter.ToInt64(il, i);
        return token;
    }

    static object? GetOperand(OperandType type, byte[] il, int i, int token, Module module)
    =>
        type switch
        {
            OperandType.InlineMethod => module.ResolveMethod(token),
            OperandType.InlineField => module.ResolveField(token),
            OperandType.InlineType => module.ResolveType(token),
            OperandType.InlineString => module.ResolveString(token),
            OperandType.InlineTok => module.ResolveMember(token),
            OperandType.InlineI => token,
            OperandType.ShortInlineI => (int)il[i],
            _ => null,
        };


    static byte OperandSize(OperandType type) =>
    type switch
    {
        OperandType.InlineNone => 0x00,
        OperandType.ShortInlineVar or OperandType.ShortInlineI or OperandType.ShortInlineBrTarget => 0x01,
        OperandType.InlineVar => 0x02,
        OperandType.InlineTok or OperandType.InlineBrTarget or OperandType.InlineField or OperandType.InlineI or OperandType.InlineMethod or OperandType.InlineSig or OperandType.InlineString or OperandType.InlineType or OperandType.ShortInlineR or OperandType.InlineSwitch => 0x04,
        OperandType.InlineR or OperandType.InlineI8 or OperandType.InlineR => 0x08
    };
}
