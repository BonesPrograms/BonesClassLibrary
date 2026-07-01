using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Text;
using BonesClassLibrary.Reflection.Collections;
using BonesClassLibrary;
using BonesClassLibrary.Bytes;
using BonesClassLibrary.Extensions;
using BonesClassLibrary.Reflection;
using HarmonyLib;
using BonesClassLibrary.IO;


MethodInfo method = AccessTools.Method("Test:UnsignedInts");
ILReader reader = new(method);
reader.PrintIL();
reader.Locals.ForEach(x => Console.WriteLine(x));

int num = -2;
byte[] bytes = num.AsBytes();
Write.To<byte>(@"C:\Users\user\Desktop\VietnamWarModLab\BonesClassLibrary\ex.txt", bytes, x => $"{x} ", false, false);


var data = MetadataMap.New(typeof(Test).Module);
var valuetuplefix = data[typeof(ValueTupleFix)]; //this stillneeds fix
foreach (var obj in valuetuplefix)
{
    Console.WriteLine(obj);
}

//so i discovered the issue
//integer -2's leading byte is 254, integer -3's leading byte is 253. it pushes -2 for uint maxvalue - 1, and -3 for uint maxvalue - 2. uint maxvalue pushes -1
//it is reading only the first byte of what is actually an int32 being pushed onto the stack
//because bytes CANNOT be represented as negative values, it is undeniably trying to push an int
//but because its operandtype, shortinlineI, only dictates reading 8 bits, it misses the other 3 bytes and truncates the value
//im not sure if its using the integer for subtraction (uint max value - integer value) to find the short's value if it's over the int32 max, or if it is
//doing bitflipping, because for the uint maxvalue, it pushing -1, and uint maxvalue - 1 != uint maxvalue


//you can actually force it to read int32 by making your value less than uintmax - byte max
///im not sure why this works because it is pushing an int both times
/// why doesn't it just arbitrarily read the first byte both times?
/// //this also isnt the result of like, overflow
/// //this bug occurs before you actually are able to overflow a byte (By decreasing its value below 0)
/// around 
///         uint insigned2 = uint.MaxValue - byte.MaxValue + 130;
///         uint insigned2 = uint.MaxValue - byte.MaxValue + 120;
/// this is the range, somewhere around here the bug occurs
/// and instead of reading int32, it decided to read a byte
/// 
/// not sure how this bug occurs, this is a direct result of the opcode's operand type value
/// if it calls for 8bits, we read 8bits, if it calls for 32bits, we read 32bits
/// 
/// its possible we are mis-identifying opcodes? maybe maybe, getting a similar opcode but getting the wrong one that calls for 8bits instead of 32
/// no definitely not, the opcode for that is single byte
/// so im not sure why its deciding to read 8 bits instead of 32 right now
/// maybe the decompiler uses context
/// it takes the first byte, then replaces the first byte of an integer with that byte, and then returns the integer
/// but it can only do that contextually after it realizes that it received a short
/// which at this current point i have literally no way of knowing if i read a short, i just get bytes and its telling me to read bytes
/// 
/// though if you think about it, its supposed to be reading int32 right, you would expect those 4 missing bytes to mess up the IL output
/// so im not sure whats going on
/// 
/// the decompiler must without a doubt be using context clues like local variable info yes
/// i think thats the key
/// if a byte value is loaded onto the stack
/// and then the very next command is pushing it into a local variable or parameter
/// i can use that to determine that it's actually supposed to be the first byte in an integer and then create that integer out of a byte array
/// furthermore, if its being inlined as a parameter in a call to a methodbase, i can use that methodbase's parameters and it's position in the stack to determine
/// oh, this byte is actually a short
/// 


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

        int integer = 1;
        gen = new();
        num = 1402105491;
        (int, char) ValueTuple = (1, 'a');

        FieldInfo info = AccessTools.Field("Test:_field");
        new AccessModifiers(info).ToString();
        AccessModifiers access = new(info);
        AccessModifiers bucess = new(info);
        access = bucess;

        List<object> list = new();
        List<object> otherlist = new();
        list = otherlist;


    }
    void UnsignedInts()
    {
        char c = '`';
        if(true)
        {

        }
        else
        {

        }
        uint unsigned = uint.MaxValue;
        uint unsigned6 = 4294967295;
        uint unsigned2 = 4294967294;
        uint unsigned87 = 4294967293;
        uint insigned2 = uint.MaxValue - byte.MaxValue + 130;
        uint insigned3 = 3294977294;
        uint insigned4 = 999999999;

        ShortTaker(4294967293);
    }

    void ShortTaker(uint iu)
    {

    }
#pragma warning restore CS0219 // Variable is assigned but its value is never used
}