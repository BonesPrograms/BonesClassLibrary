using System.Reflection;
using System.Text;
using BonesClassLibrary;
using BonesClassLibrary.Reflection;
using HarmonyLib;
using TestSpace;

MethodInfo mthd = AccessTools.Method(typeof(TokenTest), "MetaTest");
ILReader reader = new(mthd);
reader.PrintIL();


namespace TestSpace
{

    class TestObject
    {

    }

    class TokenTest
    {
        public void MetaTest()
        {
            TokenTest t = new();
            (int, int) numer = (1,1);
            UInt128 integer = 11111111;
            string obj = "";
            int num = 231244;
            DateTime time = new();
        }
    }

}