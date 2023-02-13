using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BinToAssembly;

//https://learn.microsoft.com/en-us/visualstudio/test/getting-started-with-unit-testing?view=vs-2022&tabs=dotnet%2Cmstest

namespace MultiBinaryToAssemblyConverterTests
{
    [TestClass]
    public class ParserUnitTests
    {
        [TestMethod]
        public void Test68000Parser()
        {
            Parser68000 parser = new Parser68000();
            Assert.IsNotNull(parser);
        }
        [TestMethod]
        public void TestConvertToAssembly()
        {
            Parser68000 parser = new Parser68000();
            parser.ConvertToAssembly();
        }
    }
}
