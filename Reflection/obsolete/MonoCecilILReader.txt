using System.Reflection;
using System.Text;
using HarmonyLib;
using Mono.Reflection;
using BonesClassLibrary.FileFinders;

namespace BonesClassLibrary.Reflection;


/// <summary>
/// This class uses MonoCecil to convert a method to readable IL and prints it to a file.
/// </summary>
public sealed class MonoCecilReader : ILReader
{
    private MonoCecilReader() //this class is supposed to be used like a static but it uses shared methods
    {
        
    }
    public static void PrintIL(Type type, string name, string? path = null)
    {
        PrintIL(AccessTools.Method(type, name), path);
    }
    public static void PrintIL(MethodInfo method, string? path = null)
    {
        path ??= Path.Combine(VietnamWarModLab.Path, @"BonesClassLibrary\Reflection\methodread.il");
        Instruction[] codes = [.. method.GetInstructions()];
        List<string> text = new(codes.Length);
        foreach (var code in codes)
        {
            text.Add($" {LabelSnip(code.ToString())}{code.OpCode} {OperandToString(code.Operand)}");
        }
        StreamWriter writer = new(path);
        foreach (var txt in text)
        {
            writer.WriteLine(txt);
        }
        writer.Close();
    }

    static string LabelSnip(string txt)
    {
        StringBuilder label = new();
        for (int i = 0; i < 7; i++) //idk why i put i < 7 here lol? but it works for now so well keep it. oh i see - 7 is the max character length for a jump label
        {
            label.Append(txt[i]);
        }
        label.Append(": ");
        return label.ToString();
    }


}