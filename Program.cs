using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using BonesClassLibrary;
using BonesClassLibrary.FileFinders;
using BonesClassLibrary.Reflection;
using HarmonyLib;


MethodInfo mthd = AccessTools.Method(typeof(TestType), nameof(TestType.AltSwitch));
ILParserTester.TestILParse(mthd);


class TestType
{
    class Class
    {
        public int SubField  = 3;
        public void Method()
        {

        }

    }

    public void AltSwitch()
    {
        string str = "";
        switch(str)
        {
            case "a":
            break;
            case "b":
            break;
            case "c":
            break;
        }
    }

    public void SimpleIf()
    {
        string? strng = null;
        bool value = true;
        if(value)
        {
            
        }
        else
        {
            
        }
    }
    int Field = 1;

    string Text = "Hello";

    bool Value = false;
    void Invocation(int num = 1, string obj = "h")
    {

    }

    public void LocalParamsTest(string obj, string obj2, string obj3)
    {
        int num1 = 1;
        int num2 = 2;
        int num3 = 3;
        Class cls = new();
        int num4 = cls.SubField;
        Invocation();
        cls.Method();



        bool val = false;
        bool val2 = true;
        string obj4 = "next";

        if(Field == 1)
        {
            
        }
        else if(Text == "Hello")


        while (true)
        {
            new int();
        }

    }

    public int SwitchTest()
    {
        long testlong = 24814901490;
        int i = 1;
        return i switch
        {
            1 => 2,
            3 => 4,
            5 => 6,
            _ => 10
        };
    }

    public string StringSwitch(string txt)
    {
        return txt switch
        {
            "a"=>"b",
            "c"=>"d",
            _=>"x"
        };
    }
    public void TestMethod()
    {
        if (Text == "Hello")
            Text = "Nigger";
        if (Value == true)
            return;
        if (Field > 0)
            return;
        else
            Invocation();
    }
}
