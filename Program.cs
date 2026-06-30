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

MethodInfo method = AccessTools.Method("Test:Structs");
ILReader reader = new(method);
reader.PrintIL();


var data = Metadata.MetadataMap(typeof(Test).Module);
foreach (var obj in data)
{
    if (obj.EqualsOrIsDeclaredIn(typeof(ValueTupleFix)) || obj.EqualsOrIsDeclaredIn(typeof(AccessModifiers))) //Issues: Value tuples and the Uint problem
        Console.WriteLine(obj);
}

//current il method loaded is showing a bug
// Token: 2 AsBytes: 2 
//they should not be displaying if theyre equal

//also - generic objects represented by types LocalVariableInfo and ParameterInfo need to be updated with the FixGenericString thing
//value tuples and any generic object represented by these types will be misread in IL
//though im still not sure why metadata misreads value tuple

//i can confirm though that CUSTOM structs are tokenized, im not sure about custom valuetuples tho, but defined structs are tokenized

class ValueTupleFix
{
    private ConcurrentDictionary<(int, Module), Metadata> _test;
}

class Gen<T>
{
    
}
class Test
{
    private int _field;

#pragma warning disable CS0219 // Variable is assigned but its value is never used

    void Structs<T>(int num, Gen<T> gen)
    {
        gen = new();
        num = 1402105491;
        (int, char) ValueTuple = (1, 'a');

        FieldInfo info = AccessTools.Field("Test:_field");
        AccessModifiers access = new(info);
        AccessModifiers bucess = new(info);
        access = bucess;

        List<object> list = new();
        List<object> otherlist = new();
        list = otherlist;

    }
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