using System.Collections.Generic;

namespace BinToAssembly
{
    public class PopulateOpCodeList
    {
        private List<OpCode> m_OpCodes = new List<OpCode>();
        private string processor = "";
        public string GetProcessor { get { return processor; } }

        public List<OpCode> GetOpCodes { get { return m_OpCodes; } }

        public OpCode GetOpCode(int value) { return m_OpCodes[value]; }

        private readonly XMLLoader xmlLoader = new XMLLoader();

        public void Init(
            string processor)
        {
            this.processor = processor;
            m_OpCodes.Clear();
            xmlLoader.SetValid = false;
            xmlLoader.Load(m_OpCodes, processor);
        }
    }
}
