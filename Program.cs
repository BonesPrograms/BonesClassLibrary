using System.Collections;
using System.Collections.Concurrent;
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
foreach (var obj in data)
{
    if (obj.EqualsOrIsDeclaredIn(typeof(ValueTupleFix)))
        Console.WriteLine(obj);
}

class ValueTupleFix
{
    private ConcurrentDictionary<(int, Module), Metadata> _test;
}

class Base<T>
{

}
class Inheritance : Base<Test>
{

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
#pragma warning disable CS0219 // Variable is assigned but its value is never used
    void UnsignedInts()
    {

        uint unsigned = uint.MaxValue;
        uint unsigned6 = 4294967295;
        uint unsigned2 = 4294967294;
        uint unsigned87 = 4294967293;
        uint insigned3 = 3294977294;
        uint insigned4 = 999999999;
    }
#pragma warning restore CS0219 // Variable is assigned but its value is never used
}