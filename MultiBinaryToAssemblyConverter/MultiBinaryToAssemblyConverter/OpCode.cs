using System;
using System.Reflection;

namespace BinToAssembly
{
    public class OpCode
    {
        private string m_code { get; set; }
        private bool m_illegal { get; set; }
        private string m_name = "";
        private int m_numberOfBytes = 0;
        private string m_prefix = "";
        private string m_suffix = "";
        private MethodInfo m_methodInfo;
        private string m_methodName;

        public OpCode(
            string code,
            string name,
            int numberOfBytes,
            string prefix,
            string suffix,
            string methodName,
            bool illegal
            )
        {
            m_code = code;
            m_name = name;
            m_numberOfBytes = numberOfBytes;
            m_prefix = prefix;
            m_suffix = suffix;
            m_methodName = methodName;
            Type type = typeof(OpCode);
            m_methodInfo = type.GetMethod(methodName);
            m_illegal = illegal;
        }

        public string Code { get { return m_code; } }
        public string Name { get { return m_name; } }
        public int NumberOfBytes { get { return m_numberOfBytes; } }
        public string Prefix { get { return m_prefix; } }
        public string Suffix { get { return m_suffix; } }
        public bool Illegal { get { return m_illegal; } }

        /// <summary>
        /// Build a Formated string containing the relevaent OpCode detail.
        /// </summary>
        /// <returns>The formated string.</returns>
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

        public string BNE_W(ushort[] binaryFileData)
        {
            var difference = (0xffff - binaryFileData[0]) - 1;
            return Prefix + Hex(binaryFileData) + " == " + difference.ToString();
        }

        public string BSR_W(ushort[] binaryFileData)
        {
            return Prefix + Hex(binaryFileData);
        }

        public string BSET_B(ushort[] binaryFileData)
        {
            string result = (binaryFileData[1].ToString("X4") + binaryFileData[2].ToString("X4")).ToLower();
            return Prefix + unchecked((sbyte)binaryFileData[0]).ToString("X4").ToLower() + Suffix + result;
        }

        public string BTST_B(ushort[] binaryFileData)
        {
            string result = (binaryFileData[1].ToString("X4") + binaryFileData[2].ToString("X4")).ToLower();
            return Prefix + unchecked((sbyte)binaryFileData[0]).ToString("X4").ToLower() + Suffix + result;
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
            return Prefix + binaryFileData[0].ToString("X2") + Suffix + (binaryFileData[1].ToString("X4") + binaryFileData[2].ToString("X4")).ToLower();
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
            return Prefix + Hex(binaryFileData) + Suffix;
        }

        public string MOVE_B(ushort[] binaryFileData)
        {
            string result = (binaryFileData[1].ToString("X4") + binaryFileData[2].ToString("X4")).ToLower();
            return Prefix + unchecked((sbyte)binaryFileData[0]).ToString("X2") + Suffix + result;
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
            return Prefix + result + "," + Suffix;
        }

        public string RTS(ushort[] binaryFileData)
        {
            return "";
        }

    }
}