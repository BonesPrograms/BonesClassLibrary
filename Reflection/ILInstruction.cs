using System.Reflection.Emit;

namespace BonesClassLibrary.Reflection;

public sealed class ILInstruction
{
    public readonly OpCode OpCode;
    public readonly object? Operand;
    public ILInstruction(OpCode opcode, object? operand)
    {
        OpCode = opcode;
        Operand = operand;
    }

    public override string ToString()
    {
        return $"{OpCode} {Operand}";
    }
}