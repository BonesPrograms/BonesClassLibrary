using System.Reflection;
using System.Text;

namespace BonesClassLibrary.Reflection;

/// <summary>
/// A bridge to get access modifier values from reflection objects that can't be casted into one another but share the same exact fields.
/// </summary>

public readonly struct AccessModifiers
{
    public readonly bool IsPublic;

    public readonly bool IsPrivate;

    public readonly bool IsAssembly;

    public readonly bool IsFamily;

    public readonly bool IsFamilyAndAssembly;

    public readonly bool IsFamilyOrAssembly;

    public AccessModifiers(FieldInfo field)
    {
        IsPublic = field.IsPublic;
        IsPrivate = field.IsPrivate;
        IsAssembly = field.IsAssembly;
        IsFamily = field.IsFamily;
        IsFamilyAndAssembly = field.IsFamilyAndAssembly;
        IsFamilyOrAssembly = field.IsFamilyOrAssembly;
    }

    public AccessModifiers(MethodBase mthd)
    {
        IsPublic = mthd.IsPublic;
        IsPrivate = mthd.IsPrivate;
        IsAssembly = mthd.IsAssembly;
        IsFamily = mthd.IsFamily;
        IsFamilyAndAssembly = mthd.IsFamilyAndAssembly;
        IsFamilyOrAssembly = mthd.IsFamilyOrAssembly;
    }
    public override string ToString()
    {
        if (IsPublic)
            return "public";
        else if (IsFamily)
            return "protected";
        else if (IsPrivate)
            return "private";
        else if (IsAssembly)
            return "internal";
        else if (IsFamilyAndAssembly)
            return "private protected";
        else if (IsFamilyOrAssembly)
            return "protected internal"; //this should literally never throw
        throw new InvalidOperationException("FieldInfo or MethodBase object has invalid access modifiers.");
    }
}