using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BinToAssembly
{
    public class XMLLoader
    {
        private bool valid = false;
        public bool SetValid { set { valid = value; } }

        public void Load(List<BaseOpCode> m_OpCodes, string processor)
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
                            // Split the line using the delimiter
                            string[] split = reader.Value.Split('¬');
                            string name = split[2];
                            GetDataType(name, out string dataSize);
                            if(name.Contains("BNE") || name.Contains("BEQ") || name.Contains("JSR")) 
                            {
                                m_OpCodes.Add(new Branch(split[0], split[1], name, int.Parse(split[3]), split[4], split[5], split[6], split[7], dataSize));
                            }
                            else
                            {
                                m_OpCodes.Add(new OpCode(split[0], split[1], name, int.Parse(split[3]), split[4], split[5], split[6], split[7], dataSize));
                            }

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

        private void GetDataType(string data, out string dataSize)
        {
            if (data.Contains("."))
            {
                int index = data.IndexOf(".");
                dataSize = data.Substring(index + 1);
            }
            else { dataSize = "?"; }
        }

    }
}
