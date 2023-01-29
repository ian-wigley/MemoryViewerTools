using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BinToAssembly
{
    public class Parser68xx
    {
        private int startAddress = 0;

        private List<string> code = new List<string>();
        private List<string> lineNumbers = new List<string>();
        private List<string> illegalOpcodes = new List<string>();

        private Dictionary<string, string[]> dataStatements = new Dictionary<string, string[]>();

        public Parser68xx() { }

        public void ParseFileContent(string fileName, PopulateOpCodeList populateOpCodeList, TextBox textBox)
        {
            textBox.Clear();
            var fileContent = File.ReadAllBytes(fileName);
            int filePosition = 0;
            int lineNumber = 0;
            int pc = 0;
            var m_OpCodes = populateOpCodeList.GetOpCodes;
            while (filePosition < fileContent.Length)
            {
                int opCode = fileContent[filePosition];
                lineNumber = startAddress + filePosition;
                lineNumbers.Add(lineNumber.ToString("X4"));
                string line = (startAddress + filePosition).ToString("X4");
                line += "  " + opCode.ToString("X2");
                pc = startAddress + filePosition;

                bool found = false;

                foreach (OpCode oc in m_OpCodes)
                {
                    if(opCode.ToString("X2") == "10")
                    {
                        var stop = 0;
                        opCode = opCode << 8;
                        opCode = opCode + fileContent[filePosition + 1];
                    }

                    if (oc.code == opCode.ToString("X2"))
                    {
                        ConvertToAssembly(oc, ref line, ref filePosition, fileContent, lineNumber, pc, ref dataStatements, ref illegalOpcodes);
                        //oc.GetCode(ref line, ref filePosition, fileContent, lineNumber, pc, ref dataStatements, ref illegalOpcodes);
                        found = true;
                    }
                }
                if (!found) {
                    filePosition++;
                }

                code.Add(line);
                // Testing Hack
                // filePosition++;
            }
            // Use a monospaced font
            textBox.Font = new Font(FontFamily.GenericMonospace, textBox.Font.Size);
            textBox.Lines = code.ToArray();
            //generate.Enabled = true;
            //leftWindowToolStripMenuItem.Enabled = true;
        }

        private void ConvertToAssembly (OpCode oc, ref string line, ref int filePosition, byte[] fileStuff, int lineNumber,
            int pc, ref Dictionary<string, string[]> dataStatements, ref List<string> illegalOpCodes)
        {
            string[] temp;
            if (oc.numberOfBytes == 1)
            {
                if (oc.illegal)
                {
                    // Add the programme counter location to the list of illegal opcodes found
                    illegalOpCodes.Add(pc.ToString("X4"));
                }

                temp = new string[1] { "!byte $" + oc.code };
                dataStatements.Add(pc.ToString("X4"), temp);
                line += "          " + oc.name;
                filePosition += 1;
            }
            if (oc.numberOfBytes == 2)
            {
                if (oc.illegal)
                {
                    illegalOpCodes.Add(pc.ToString("X4"));
                }
                temp = new string[2];
                temp[0] = "!byte $" + oc.code;
                temp[1] = "!byte $" + fileStuff[filePosition + 1].ToString("X2");
                dataStatements.Add(pc.ToString("X4"), temp);
                line += " " + fileStuff[filePosition + 1].ToString("X2");

                if (oc.name.Contains("BCC") || oc.name.Contains("BCS") ||
                    oc.name.Contains("BEQ") || oc.name.Contains("BMI") ||
                    oc.name.Contains("BNE") || oc.name.Contains("BPL") ||
                    oc.name.Contains("BVC") || oc.name.Contains("BVS"))
                {
                    sbyte s = unchecked((sbyte)fileStuff[filePosition + 1]);
                    s += 2;
                    line += "       " + oc.name + " " + oc.prefix + (pc + s).ToString("X4");
                }
                else
                {
                    line += "       " + oc.name + " " + oc.prefix + fileStuff[filePosition + 1].ToString("X2") + oc.suffix;
                }
                filePosition += 2;
            }
            else if (oc.numberOfBytes == 3 && (filePosition < fileStuff.Length - 3))
            {
                if (oc.illegal)
                {
                    illegalOpCodes.Add(pc.ToString("X4"));
                }

                temp = new string[3] { "!byte $" + oc.code, "!byte $" + fileStuff[filePosition + 1].ToString("X2"), "!byte $" + fileStuff[filePosition + 2].ToString("X2") };
                //temp[0] = "!byte $" + oc.code;
                //temp[1] = "!byte $" + fileStuff[filePosition + 1].ToString("X2");
                //temp[2] = "!byte $" + fileStuff[filePosition + 2].ToString("X2");
                dataStatements.Add(pc.ToString("X4"), temp);

                line += " " + fileStuff[filePosition + 1].ToString("X2") + " " + fileStuff[filePosition + 2].ToString("X2");
                //line += "    " + oc.name + " " + oc.prefix + fileStuff[filePosition + 2].ToString("X2") + fileStuff[filePosition + 1].ToString("X2") + oc.suffix;
                line += "    " + oc.name + " " + oc.prefix + fileStuff[filePosition + 1].ToString("X2") + fileStuff[filePosition + 2].ToString("X2") + oc.suffix;
                filePosition += 3;
            }
            else if (oc.numberOfBytes == 4 && (filePosition < fileStuff.Length - 4))
            {
                temp = new string[4] { "!byte $" + oc.code, "!byte $" + fileStuff[filePosition + 1].ToString("X2"), "!byte $" + fileStuff[filePosition + 2].ToString("X2"), "!byte $" + oc.code };
                line += " " + fileStuff[filePosition + 1].ToString("X2") + " " + fileStuff[filePosition + 2].ToString("X2");
                line += "    " + oc.name + " " + oc.prefix + fileStuff[filePosition + 2].ToString("X2") + fileStuff[filePosition + 3].ToString("X2") + oc.suffix;
                filePosition += 4;

            }
            else if (oc.numberOfBytes == 3 && (filePosition == fileStuff.Length - 2))
            {
                filePosition = fileStuff.Length;
            }
        }
    }

 //// opcode ce ldu is a 3 byter...
}//// opcode cc ldd is a 3 byter...