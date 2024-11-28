﻿using System.Reflection;

namespace BinToAssembly
{
    public class BaseOpCode
    {
        protected int m_numberOfBytes = 0;
        protected string m_name = "";
        protected string m_prefix = "";
        protected string m_firstfix = "";
        protected string m_midfix = "";
        protected string m_suffix = "";
        protected MethodInfo m_methodInfo;
        protected string m_methodName;
        protected string pad = "    ";

        public BaseOpCode() { }

        public string m_code { get; set; }
        public bool m_illegal { get; set; }
        public string Code { get { return m_code; } }
        public string Name { get { return m_name; } }
        public int NumberOfBytes { get { return m_numberOfBytes; } }
        public string Prefix { get { return m_prefix; } }
        public string Firstfix { get { return m_firstfix; } }
        public string Midfix { get { return m_midfix; } }
        public string Suffix { get { return m_suffix; } }
        public bool Illegal { get { return m_illegal; } }


        public string Detail(ref int filePosition, byte[] binaryFileData)
        {
            string elementOne = "";
            string elementTwo = "";
            string elementThree = "";

            filePosition += 2;

            if (NumberOfBytes == 2)
            {
                elementOne = ((short)binaryFileData[filePosition++]).ToString("X2") +
                    ((short)binaryFileData[filePosition++]).ToString("X2");
            }

            if (NumberOfBytes == 4)
            {
                elementOne = ((short)binaryFileData[filePosition++]).ToString("X2") +
                ((short)binaryFileData[filePosition++]).ToString("X2");
                elementTwo = ((short)binaryFileData[filePosition++]).ToString("X2") +
                    ((short)binaryFileData[filePosition++]).ToString("X2");
            }

            if (NumberOfBytes == 6)
            {
                elementOne = ((short)binaryFileData[filePosition++]).ToString("X2") +
                            ((short)binaryFileData[filePosition++]).ToString("X2");
                elementTwo = ((short)binaryFileData[filePosition++]).ToString("X2") +
                               ((short)binaryFileData[filePosition++]).ToString("X2");
                elementThree = ((short)binaryFileData[filePosition++]).ToString("X2") +
                               ((short)binaryFileData[filePosition++]).ToString("X2");
            }

            // Temporary fixes
            if (NumberOfBytes == 8)
            {
                filePosition += 8;
            }
            if (NumberOfBytes == 10)
            {
                filePosition += 10;
            }
            if (NumberOfBytes == 12)
            {
                filePosition += 12;
            }
            //if (elementOne.Contains("48E7"))
            //{
            //    elementTwo = "";
            //}


            string binOne = !elementOne.Equals("") ? elementOne : pad;
            string binTwo = !elementTwo.Equals("") ? elementTwo : pad;

            string retunLine = " " + binOne + " " + binTwo + "          " + Name + " " + Prefix + elementOne + Firstfix + elementTwo + Midfix + elementThree + Suffix;
            return retunLine;
        }
    }
}
