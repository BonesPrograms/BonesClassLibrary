using System.Collections;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Text;
using BonesClassLibrary;
using BonesClassLibrary.Reflection;
using HarmonyLib;

var map = Metadata.Get(typeof(Test));
Console.WriteLine(map);

static byte[] TwoGigArray()
{
    return new byte[Array.MaxLength];
}
class Test() : IEnumerable
{

    public IEnumerator GetEnumerator()
    {
        yield return new();
    }
    public void Method()
    {
        decimal dec = 2421555.3242M;
        List<string> list = new();
    }
}