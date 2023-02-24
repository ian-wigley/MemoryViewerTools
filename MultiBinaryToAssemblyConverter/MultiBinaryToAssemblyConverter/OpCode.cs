using System.Collections.Generic;

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

        public OpCode(
            string code,
            string name,
            int numberOfBytes,
            string prefix,
            string suffix,
            bool illegal)
        {
            m_code = code;
            m_name = name;
            m_numberOfBytes = numberOfBytes;
            m_prefix = prefix;
            m_suffix = suffix;
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
        public string Format(ref int filePosition, ref short[] binaryFileData)
        {
            filePosition += 1;
            string hex = "";

            for (int i = 0; i < binaryFileData.Length; i++)
            {
                hex += binaryFileData[i].ToString("X4");
            }
            return "       " + Name + " " + Prefix + hex;
        }
    }
}