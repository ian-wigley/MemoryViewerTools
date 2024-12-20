﻿using System.Collections.Generic;
using System.Linq;

namespace BinToAssembly
{
    public class PopulateOpCodeList
    {
        private readonly List<BaseOpCode> m_OpCodes = new List<BaseOpCode>();
        public List<BaseOpCode> GetOpCodes { get { return m_OpCodes; } }
        private readonly XMLLoader xmlLoader = new XMLLoader();
        public XMLLoader GetXMLLoader { get { return xmlLoader; } }
        private string processor = "";
        public string GetProcessor { get { return processor; } }

        public void Init(string processor)
        {
            this.processor = processor;
            m_OpCodes.Clear();
            xmlLoader.SetValid = false;
            xmlLoader.LoadSettings(processor);
            xmlLoader.LoadOpCodes(m_OpCodes, processor);
        }

        //public dynamic GetOpCode(string value)
        //{
        //    return m_OpCodes.FirstOrDefault(opCode => opCode.Code.Equals(value));
        //}

        public dynamic GetOpCode(string t, string tt)
        {
            return m_OpCodes.FirstOrDefault(opCode => opCode.m_codeOne.Equals(t) && opCode.m_codeTwo.Equals(tt));

        }
    }
}
