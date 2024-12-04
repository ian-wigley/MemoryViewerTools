using System;

namespace BinToAssembly
{
    public class Branch : BaseOpCode
    {
        public Branch(
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

        public new string Detail(ref int filePosition, byte[] binaryFileData)
        {
            string elementOne = "";
            string elementTwo = "";
            string elementThree = "";

            if (NumberOfBytes == 1)
            {
                sbyte amount = unchecked((sbyte)binaryFileData[filePosition + 1]);
                int result = filePosition + 2 + amount;
                elementOne = result.ToString("X4");
                filePosition += 2;
            }

            if (NumberOfBytes == 2)
            {
                filePosition += 2;
                elementOne = ((short)binaryFileData[filePosition++]).ToString("X2") +
                    ((short)binaryFileData[filePosition++]).ToString("X2");
                var amount = Convert.ToInt16(elementOne, 16);
                int result = filePosition - NumberOfBytes + amount;
                elementOne = result.ToString("X4");
            }

            string binOne = !elementOne.Equals("") ? elementOne : pad;
            string binTwo = !elementTwo.Equals("") ? elementTwo : pad;

            string retunLine = " " + binOne + " " + binTwo + "          " + Name + " " + Prefix + elementOne + Firstfix + elementTwo + Midfix + elementThree + Suffix;
            return retunLine;
        }
    }
}
