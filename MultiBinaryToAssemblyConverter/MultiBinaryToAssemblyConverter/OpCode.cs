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

        public OpCode(string code, string name, int numberOfBytes, string prefix, string suffix, bool illegal)
        {
            m_code = code;
            m_name = name;
            m_numberOfBytes = numberOfBytes;
            m_prefix = prefix;
            m_suffix = suffix;
            m_illegal = illegal;
        }

        public string code { get { return m_code; } }
        public string name { get { return m_name; } }
        public int numberOfBytes { get { return m_numberOfBytes; } }
        public string prefix { get { return m_prefix; } }
        public string suffix { get { return m_suffix; } }
        public bool illegal { get { return m_illegal; } }
  
    }
}