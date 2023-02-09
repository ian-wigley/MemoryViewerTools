using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BinToAssembly
{
    public class XMLLoader
    {
        private bool valid = false;
        public bool SetValid { set { valid = value; } }

        public void Load(List<OpCode> m_OpCodes, string processor)
        {
            string xmlOpCodes = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()) + "\\" + processor + "-codes.xml";
            XmlTextReader reader = new XmlTextReader(xmlOpCodes);
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    // The node is an element.
                    case XmlNodeType.Element:
                        break;
                    // Display the text in each element.
                    case XmlNodeType.Text:
                        if (valid)
                        {
                            string[] split = reader.Value.Split('.');
                            m_OpCodes.Add(new OpCode(split[0], split[1], int.Parse(split[2]), split[3], split[4], bool.Parse(split[5])));
                        }

                        if (reader.Value.Equals(processor))
                        {
                            valid = true;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }
        }

    }
}
