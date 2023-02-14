using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BinToAssembly;

//https://learn.microsoft.com/en-us/visualstudio/test/getting-started-with-unit-testing?view=vs-2022&tabs=dotnet%2Cmstest

namespace MultiBinaryToAssemblyConverterTests
{
    [TestClass]
    public class ParserUnitTests
    {
        private PopulateOpCodeList InitOpCodeList()
        {
            PopulateOpCodeList populateOpCodeList = new PopulateOpCodeList();
            populateOpCodeList.Init("68000");
            return populateOpCodeList;
        }

        [TestMethod]
        public void Test68000Parser()
        {
            Parser68000 parser = new Parser68000();
            Assert.IsNotNull(parser);
        }

        [TestMethod]
        public void TestRTSOpcode()
        {
            int opCode = 20085;
            var oc = InitOpCodeList().GetOpCode(opCode.ToString("X4"));
            Assert.IsTrue(oc.name == "RTS");
            Assert.IsTrue(oc.code == "4E75");
        }
    }
}
