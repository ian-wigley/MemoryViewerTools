using System;

namespace BinToAssembly
{
    public class OpCode : BaseOpCode
    {
        public OpCode(
            string codeOne,
            string codeTwo,
            string name,
            int numberOfBytes,
            string prefix,
            string firstfix,
            string midfix,
            string suffix,
            string methodName
            )
        {
            m_codeOne = codeOne;
            m_codeTwo = codeTwo;
            m_code = codeOne + codeTwo;
            m_name = name;
            m_numberOfBytes = numberOfBytes;
            m_prefix = prefix;
            m_firstfix = firstfix;
            m_midfix = midfix;
            m_suffix = suffix;
            m_methodName = methodName;
            Type type = typeof(OpCode);
            m_methodInfo = type.GetMethod(methodName);
        }
    }
}