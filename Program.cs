using System.Collections;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Text;
using BonesClassLibrary;
using BonesClassLibrary.Bytes;
using BonesClassLibrary.Extensions;
using BonesClassLibrary.Reflection;
using HarmonyLib;

new ILReader(AccessTools.Method(typeof(Test), "Switching")).PrintIL();

var data = Metadata.MetadataMap(typeof(Test).Module);
foreach(var obj in data)
{
    Console.WriteLine("\nREADING TYPE!");
    Console.WriteLine(obj.Key);
    foreach(var ob in obj.Value)
    Console.WriteLine(ob);
}
class Test
{

    public int Switching() //im not seeing hte 'swithc' opcode!!!!! check my other switches
    {
        int vibe = 5;
        switch (vibe)
        {
            case 3:
                break;
            case 2:
                break;
        }
        return 55 switch
        {
            1 => 2,
            3 => 4,
            _ => default
        };
    }
    public void Method() //this is for researching how to fix uints not displaying properly in my IL
    {
        short shorter = 32424;
        uint unsigned = uint.MaxValue;
        uint unsigned6 = 4294967295;
        uint unsigned2 = 4294967294;
        uint unsigned87 = 4294967293;
        uint insigned3 = 3294977294;
        uint insigned4 = 999999999;
        int signed0 = int.MaxValue;
        int signed = 2147483647;
    }
}