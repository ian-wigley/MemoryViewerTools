using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BinToAssembly
{
    public class Parser6502
    {
        private int startAddress = 0;
        private List<string> illegalOpcodes = new List<string>();
        private Dictionary<string, string[]> dataStatements = new Dictionary<string, string[]>();

        public Parser6502() {}

        public void ParseFileContent(string fileName, PopulateOpCodeList populateOpCodeList, TextBox textBox,
            ref List<string> lineNumbers, ref List<string> code)
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
                foreach (OpCode oc in m_OpCodes)
                {
                    if (oc.Code == opCode.ToString("X2"))
                    {
                        ConvertToAssembly(oc, ref line, ref filePosition, fileContent, lineNumber, pc, ref dataStatements, ref illegalOpcodes);
                        //oc.GetCode(          ref line, ref filePosition, fileContent, lineNumber, pc, ref dataStatements, ref illegalOpcodes);
                    }
                }
                code.Add(line);
            }
            // Use a monospaced font
            textBox.Font = new Font(FontFamily.GenericMonospace, textBox.Font.Size);
            textBox.Lines = code.ToArray();
            //generate.Enabled = true;
            //leftWindowToolStripMenuItem.Enabled = true;
        }

        private void ConvertToAssembly(OpCode oc, ref string line, ref int filePosition, byte[] fileStuff, int lineNumber,
            int pc, ref Dictionary<string, string[]> dataStatements, ref List<string> illegalOpCodes)
        {
            string[] temp;
            if (oc.NumberOfBytes == 1)
            {
                if (oc.Illegal)
                {
                    // Add the programme counter location to the list of illegal opcodes found
                    illegalOpCodes.Add(pc.ToString("X4"));
                }

                temp = new string[1] { "!byte $" + oc.Code };
                dataStatements.Add(pc.ToString("X4"), temp);
                line += "          " + oc.Name;
                filePosition += 1;
            }
            if (oc.NumberOfBytes == 2)
            {
                if (oc.Illegal)
                {
                    illegalOpCodes.Add(pc.ToString("X4"));
                }
                temp = new string[2];
                temp[0] = "!byte $" + oc.Code;
                temp[1] = "!byte $" + fileStuff[filePosition + 1].ToString("X2");
                dataStatements.Add(pc.ToString("X4"), temp);
                line += " " + fileStuff[filePosition + 1].ToString("X2");

                if (oc.Name.Contains("BCC") || oc.Name.Contains("BCS") ||
                    oc.Name.Contains("BEQ") || oc.Name.Contains("BMI") ||
                    oc.Name.Contains("BNE") || oc.Name.Contains("BPL") ||
                    oc.Name.Contains("BVC") || oc.Name.Contains("BVS"))
                {
                    sbyte s = unchecked((sbyte)fileStuff[filePosition + 1]);
                    s += 2;
                    line += "       " + oc.Name + " " + oc.Prefix + (pc + s).ToString("X4");
                }
                else
                {
                    line += "       " + oc.Name + " " + oc.Prefix + fileStuff[filePosition + 1].ToString("X2") + oc.Suffix;
                }
                filePosition += 2;
            }
            else if (oc.NumberOfBytes == 3 && (filePosition < fileStuff.Length - 3))
            {
                if (oc.Illegal)
                {
                    illegalOpCodes.Add(pc.ToString("X4"));
                }

                temp = new string[3];
                temp[0] = "!byte $" + oc.Code;
                temp[1] = "!byte $" + fileStuff[filePosition + 1].ToString("X2");
                temp[2] = "!byte $" + fileStuff[filePosition + 2].ToString("X2");
                dataStatements.Add(pc.ToString("X4"), temp);

                line += " " + fileStuff[filePosition + 1].ToString("X2") + " " + fileStuff[filePosition + 2].ToString("X2");
                line += "    " + oc.Name + " " + oc.Prefix + fileStuff[filePosition + 2].ToString("X2") + fileStuff[filePosition + 1].ToString("X2") + oc.Suffix;
                filePosition += 3;
            }
            else if (oc.NumberOfBytes == 3 && (filePosition == fileStuff.Length - 2))
            {
                filePosition = fileStuff.Length;
            }
        }
    }
}