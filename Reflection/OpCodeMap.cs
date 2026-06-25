using System.Reflection.Emit;
using System.Reflection;
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