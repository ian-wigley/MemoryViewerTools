using System;
using System.Reflection;

namespace BinToAssembly
{
    public class OpCode
    {
        private string m_name = "";
        private int m_numberOfBytes = 0;
        private string m_prefix = "";
        private string m_firstfix = "";
        private string m_midfix = "";
        private string m_suffix = "";
        private MethodInfo m_methodInfo;
        private string m_methodName;

        public OpCode(
            string code,
            string name,
            int numberOfBytes,
            string prefix,
            string firstfix,
            string midfix,
            string suffix,
            string methodName
            )
        {
            m_code = code;
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

        private string m_code { get; set; }
        private bool m_illegal { get; set; }
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
            string elementFour = "";

            elementOne = ((short)binaryFileData[filePosition++]).ToString("X2") +
                    ((short)binaryFileData[filePosition++]).ToString("X2");

            if (NumberOfBytes == 2)
            {
                elementTwo = ((short)binaryFileData[filePosition++]).ToString("X2") +
                    ((short)binaryFileData[filePosition++]).ToString("X2");
            }

            if (NumberOfBytes == 4)
            {
                elementTwo = ((short)binaryFileData[filePosition++]).ToString("X2") +
                ((short)binaryFileData[filePosition++]).ToString("X2");
                elementThree = ((short)binaryFileData[filePosition++]).ToString("X2") +
                    ((short)binaryFileData[filePosition++]).ToString("X2");
            }

            if (NumberOfBytes == 6)
            {
                elementTwo = ((short)binaryFileData[filePosition++]).ToString("X2") +
                            ((short)binaryFileData[filePosition++]).ToString("X2");
                elementThree = ((short)binaryFileData[filePosition++]).ToString("X2") +
                               ((short)binaryFileData[filePosition++]).ToString("X2");
                elementFour = ((short)binaryFileData[filePosition++]).ToString("X2") +
                               ((short)binaryFileData[filePosition++]).ToString("X2");
            }

            // Temporary fixes

            if (elementOne.Contains("48E7"))
            {
                elementTwo = "";
            }

            string retunLine = "          " + Name + " " + Prefix + elementTwo + Firstfix + elementThree + Midfix + elementFour + Suffix;
            return retunLine;
        }


        /// <summary>
        /// Build a Formatted string containing the relevant OpCode detail.
        /// </summary>
        /// <returns>The formatted string.</returns>
        public string Format(ref int filePosition, ushort[] binaryFileData)
        {
            filePosition += 1;
            string detail = "";

            // If additional formatting is required, bounce out to the relevant method.
            if (m_methodInfo != null)
            {
                var parameters = new object[] { binaryFileData };
                detail += (string)m_methodInfo.Invoke(this, parameters);
            }

            return "       " + Name + " " + detail;
        }

        public string Hex(ushort[] binaryFileData)
        {
            string hex = "";
            for (int i = 0; i < binaryFileData.Length; i++)
            {
                hex += binaryFileData[i].ToString("X4");
            }
            return hex.ToLower();
        }

        public string ABCD(ushort[] binaryFileData)
        {
            return "";
        }

        public string ADD(ushort[] binaryFileData)
        {
            return "";
        }

        public string ADDQ(ushort[] binaryFileData)
        {
            return "";
        }

        public string AND(ushort[] binaryFileData)
        {
            return "";
        }

        public string ASL(ushort[] binaryFileData)
        {
            return "";
        }

        public string BCC(ushort[] binaryFileData)
        {
            return "";
        }

        public string BCHG(ushort[] binaryFileData)
        {
            return "";
        }

        public string BEQ(ushort[] binaryFileData)
        {
            return "";
        }

        public string BMIQ(ushort[] binaryFileData)
        {
            return "";
        }

        public string BNE_W(ushort[] binaryFileData)
        {
            var difference = (0xffff - binaryFileData[0]) - 1;
            return Prefix + Hex(binaryFileData) + " == " + difference.ToString();
        }

        public string BRA(ushort[] binaryFileData)
        {
            return "";
        }

        public string BSR_W(ushort[] binaryFileData)
        {
            return Prefix + Hex(binaryFileData);
        }

        public string BSET_B(ushort[] binaryFileData)
        {
            string result = (binaryFileData[1].ToString("X4") + binaryFileData[2].ToString("X4")).ToLower();
            return Prefix + unchecked((sbyte)binaryFileData[0]).ToString("X4").ToLower() + Midfix + result;
        }

        public string BTST_B(ushort[] binaryFileData)
        {
            string result = (binaryFileData[1].ToString("X4") + binaryFileData[2].ToString("X4")).ToLower();
            return Prefix + unchecked((sbyte)binaryFileData[0]).ToString("X4").ToLower() + Midfix + result;
        }

        public string CLR_W(ushort[] binaryFileData)
        {
            return "";
        }

        public string CLR_L(ushort[] binaryFileData)
        {
            return "";
        }

        public string CMP_B(ushort[] binaryFileData)
        {
            return Prefix + binaryFileData[0].ToString("X2") + Midfix + (binaryFileData[1].ToString("X4") + binaryFileData[2].ToString("X4")).ToLower();
        }

        public string DBF(ushort[] binaryFileData)
        {
            return "";
        }

        public string EOR(ushort[] binaryFileData)
        {
            return "";
        }

        public string JSR(ushort[] binaryFileData)
        {
            if (binaryFileData.Length == 1)
            {
                return "-132,(A6)";
            }
            else
            {
                return Prefix + Hex(binaryFileData);
            }
        }

        public string LEA_L(ushort[] binaryFileData)
        {
            return Prefix + Hex(binaryFileData) + Midfix;
        }

        public string LSR_W(ushort[] binaryFileData)
        {
            return "";
        }

        public string MOVE_B(ushort[] binaryFileData)
        {
            string result = (binaryFileData[1].ToString("X4") + binaryFileData[2].ToString("X4")).ToLower();
            return Prefix + unchecked((sbyte)binaryFileData[0]).ToString("X2") + Midfix + result;
        }

        public string MOVE_W(ushort[] binaryFileData)
        {
            return Prefix + binaryFileData[0].ToString("X4") + "," + binaryFileData[1].ToString("X2") + "(A0)";
        }

        public string MOVE_L(ushort[] binaryFileData)
        {
            string result = (binaryFileData[0].ToString("X4") + binaryFileData[1].ToString("X4")).ToLower();
            return Prefix + result + "," + binaryFileData[2].ToString("X2") + "(A0)";
        }

        public string MOVEA_L(ushort[] binaryFileData)
        {
            string result = (binaryFileData[0].ToString("X4") + binaryFileData[1].ToString("X4")).ToLower();
            return Prefix + result + "," + Midfix;
        }

        public string MULU(ushort[] binaryFileData)
        {
            return "";
        }

        public string NOT(ushort[] binaryFileData)
        {
            return "";
        }

        public string OR(ushort[] binaryFileData)
        {
            return "";
        }

        public string PEA_L(ushort[] binaryFileData)
        {
            return "";
        }

        public string ROR(ushort[] binaryFileData)
        {
            return "";
        }

        public string SWAP(ushort[] binaryFileData)
        {
            return "";
        }

        public string TST(ushort[] binaryFileData)
        {
            return "";
        }
    }
}