using System.Reflection;
using System.Text;
using BonesClassLibrary;
using BonesClassLibrary.Reflection;
using HarmonyLib;
using TestSpace;

MethodInfo mthd = AccessTools.Method(typeof(ConsoleHelper), "Choices");
ILReader reader = new(mthd);
reader.PrintIL();


namespace TestSpace
{

    class TestObject
    {

    }

    class TokenTest
    {
        public int MetaTest()
        {
            int i = 0;
            return i switch
            {
                1 => 2,
                2=>3,
            };
        }
    }

}