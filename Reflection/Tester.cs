using System.Reflection;
using BonesClassLibrary.FileFinders;

namespace BonesClassLibrary.Reflection;

/// <summary>
/// Currently only for console apps, uses console for log.
/// </summary>
public static class ILParserTester
{
    public static void TestILParse(MethodInfo mthd, string? path = null)
    {
        path ??= Path.Combine(VietnamWarModLab.Path, @"BonesClassLibrary\Reflection\methodread.il");
        ReadBytes(mthd.GetMethodBody()); //not rly necessary anymore
        List<ILInstruction> codes = new ILParser(mthd).GetIL();
        Console.WriteLine("Reading method opcodes");
        codes.ToList().ForEach(Console.WriteLine); //this here is MY shit, we are testing to see if this is good
        Console.WriteLine("Reading actual IL to file!");
        MonoCecilReader.PrintIL(mthd); //this is monocecil, we are testing to see if my shit is as good as monocecil's reader
    }

    static void ReadBytes(MethodBody? body)
    {
        if (body != null)
        {
            byte[]? arr = body.GetILAsByteArray();
            if (arr != null)
            {
                Console.WriteLine("Reading bytes");
                arr.Select(x => x.ToString()).ToList().ForEach(Console.WriteLine);
            }
        }
        else
            throw new NullReferenceException("MethodBody is null!");
    }
}