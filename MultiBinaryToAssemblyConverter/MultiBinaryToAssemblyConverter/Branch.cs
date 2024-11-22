using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinToAssembly
{
    public class Branch : OpCode
    {
        public Branch(
            string code,
            string name,
            int numberOfBytes,
            string prefix,
            string firstfix,
            string midfix,
            string suffix,
            string methodName
            ) : base(code, name, numberOfBytes, prefix, firstfix, midfix, suffix, methodName)
        {
            //m_code = code;
            //m_name = name;
            //m_numberOfBytes = numberOfBytes;
            //m_prefix = prefix;
            //m_firstfix = firstfix;
            //m_midfix = midfix;
            //m_suffix = suffix;
            //m_methodName = methodName;
            //Type type = typeof(OpCode);
            //m_methodInfo = type.GetMethod(methodName);
        }


        public new string Detail(ref int filePosition, byte[] binaryFileData)
        {
            string elementOne = "";
            string elementTwo = "";
            string elementThree = "";
            string elementFour = "";
            return "";
        }
    }
}
