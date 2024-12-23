using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinToAssembly
{
    public class Move : BaseOpCode
    {
        public Move(
            string codeOne,
            string codeTwo,
            string name,
            int numberOfBytes,
            string prefix,
            string firstfix,
            string midfix,
            string suffix,
            string dataSize
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
            m_dataSize = dataSize;
        }

        public new string Detail(ref int filePosition, byte[] binaryFileData)
        {
            string elementOne = "";
            string elementTwo = "";
            string elementThree = "";
            string binOne = pad;
            string binTwo = pad;

            string retunLine = " " + binOne + " " + binTwo + "          " + Name + " " + Prefix + elementOne + Firstfix + elementTwo + Midfix + elementThree + Suffix;
            return retunLine;
        }

    }
}
