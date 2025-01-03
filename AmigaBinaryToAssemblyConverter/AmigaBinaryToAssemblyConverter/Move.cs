﻿namespace BinToAssembly
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

        /// <summary>
        ///
        /// </summary>
        public new string Detail(ref int filePosition, byte[] binaryFileData)
        {
            string elementOne = "";
            string elementTwo = "";
            string elementThree = "";

            filePosition += 2;

            if (NumberOfBytes == 2)
            {
                elementOne = GetTwoShorts(ref filePosition, binaryFileData);
            }

            if (NumberOfBytes == 4)
            {
                elementOne = GetTwoShorts(ref filePosition, binaryFileData);
                elementTwo = GetTwoShorts(ref filePosition, binaryFileData);
            }

            if (NumberOfBytes == 6)
            {
                elementOne = GetTwoShorts(ref filePosition, binaryFileData);
                elementTwo = GetTwoShorts(ref filePosition, binaryFileData);
                elementThree = GetTwoShorts(ref filePosition, binaryFileData);
            }

            // Temporary fixes
            if (NumberOfBytes == 8)
            {
                elementOne = GetTwoShorts(ref filePosition, binaryFileData);
                elementTwo = GetTwoShorts(ref filePosition, binaryFileData);
                elementThree = GetTwoShorts(ref filePosition, binaryFileData) + GetTwoShorts(ref filePosition, binaryFileData);
            }
            if (NumberOfBytes == 10)
            {
                filePosition += 10;
            }
            if (NumberOfBytes == 12)
            {
                filePosition += 12;
            }

            string binOne = !elementOne.Equals("") ? elementOne : pad;
            string binTwo = !elementTwo.Equals("") ? elementTwo : pad;

            if (Code.Equals("23E9"))
            {
                elementTwo = "";
            }
            if (Code.Equals("33F0"))
            {
                elementOne = "";
            }
            if (Code.Equals("48E7"))
            {
                elementOne = "";
            }
            string returnLine = " " + binOne + " " + binTwo + "          " + Name + " " + Prefix + elementOne + Firstfix + elementTwo + Midfix + elementThree + Suffix;
            return returnLine;
        }

    }
}
