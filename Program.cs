using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using BonesClassLibrary;
using BonesClassLibrary.FileFinders;
using BonesClassLibrary.Reflection;
using HarmonyLib;


MethodInfo mthd = AccessTools.Method(typeof(TestType), "SwitchTest");
ILParserTester.TestILParse(mthd);

class TestType
{
    int Field = 1;

    string Text = "Hello";

    bool Value = false;
    void MethodCall()
    {
        
    }

    int SwitchTest()
    {
        long testlong = 24814901490;
        int i = 1;
       return i switch
       {
           1=>2,
           3=>4,
           5=>6,
           _=>10
       } ;
    }
    void TestMethod()
    {
        if(Text == "Hello")
        Text = "Nigger";
        if(Value == true)
        return;
        if(Field > 0)
        return;
        else
        MethodCall();
    }
}
