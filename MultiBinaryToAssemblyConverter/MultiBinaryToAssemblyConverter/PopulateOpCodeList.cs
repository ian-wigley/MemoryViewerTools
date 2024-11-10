using System.Collections.Generic;
using System.Linq;

namespace BinToAssembly
{
    public class PopulateOpCodeList
    {
        private readonly List<OpCode> m_OpCodes = new List<OpCode>();
        public List<OpCode> GetOpCodes { get { return m_OpCodes; } }
        private readonly XMLLoader xmlLoader = new XMLLoader();
        private string processor = "";
        public string GetProcessor { get { return processor; } }

        public OpCode GetOpCode(string value)
        {
            return m_OpCodes.FirstOrDefault(opCode => opCode.Code.Equals(value));
        }

        public void Init(string processor)
        {
            this.processor = processor;
            m_OpCodes.Clear();
            xmlLoader.SetValid = false;
            xmlLoader.Load(m_OpCodes, processor);
        }
    }
}
