using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BinToAssembly
{
    public class Parser68000
    {

        private int startAddress = 0;
        private List<string> illegalOpcodes = new List<string>();
        private Dictionary<string, string[]> dataStatements = new Dictionary<string, string[]>();

        public byte[] LoadData(string fileName)
        {
            try
            {
                return File.ReadAllBytes(fileName);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occured whilst loading data", exception.Message);
                return new byte[0];
            }
        }

        public void ParseFileContent(
            byte[] data,
            PopulateOpCodeList populateOpCodeList,
            TextBox textBox,
            ref List<string> lineNumbers,
            ref List<string> code
            )
        {
            textBox.Clear();
            int filePosition = 0;
            int lineNumber = 0;
            int pc = 0;
            var m_OpCodes = populateOpCodeList.GetOpCodes;
            while (filePosition < data.Length - 4)
            {

                string rte = "0100111001110011";
                string strHex = Convert.ToInt32(rte, 2).ToString("X");
                string rts = "0100111001110101";

                // lookup the first 4 bits of each byte (Nybble)
                //int opCode = fileContent[filePosition];// << 2 + fileContent[filePosition+1];
                int t = data[filePosition] << 8;
                int opCode = t + data[filePosition + 1]; //<- opCode 

                // Opcodes come from 2 bytes
                if (filePosition == 32)
                {
                    //byte bytes = System.Convert.ToByte(0x0839);
                    byte[] bytes = BitConverter.GetBytes(0x0839);
                    //[0] = 57
                    //[1] = 8
                    //[2] = 0
                    //[3] = 0
                    // 68k is Big Endian
                    // Increment by 2 bytes
                    var x = opCode.ToString("X4");
                }


                lineNumber = startAddress + filePosition;
                lineNumbers.Add(lineNumber.ToString("X4"));
                string line = (startAddress + filePosition).ToString("X4");
                line += "  " + opCode.ToString("X4"); // 2");
                pc = startAddress + filePosition;

                bool found = false;

                foreach (OpCode oc in m_OpCodes)
                {
                    if (oc.code == opCode.ToString("X4")) //X2"))
                    {
                        ConvertToAssembly(oc, ref line, ref filePosition, data, lineNumber, pc, ref dataStatements, ref illegalOpcodes);
                        found = true;
                    }
                }
                code.Add(line);
                if (!found)
                {
                    filePosition += 2;
                }
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
                filePosition += 2; // 1;
            }
            if (oc.numberOfBytes == 2)
            {
                if (oc.illegal)
                {
                    illegalOpCodes.Add(pc.ToString("X4"));
                }
                temp = new string[2] { "!byte $" + oc.code, "!byte $" + fileStuff[filePosition + 1].ToString("X2") };
                //temp[0] = "!byte $" + oc.code;
                //temp[1] = "!byte $" + fileStuff[filePosition + 1].ToString("X2");
                dataStatements.Add(pc.ToString("X4"), temp);
                //line += " " + fileStuff[filePosition + 1].ToString("X2");
                line += " " + fileStuff[filePosition + 2].ToString("X2");

                if (oc.name.Contains("JSR"))
                {
                    sbyte s1 = unchecked((sbyte)fileStuff[filePosition + 2]);
                    sbyte s2 = unchecked((sbyte)fileStuff[filePosition + 3]);

                    byte t1 = unchecked(fileStuff[filePosition + 2]);
                    byte t2 = unchecked(fileStuff[filePosition + 3]);
                    Int16 i = BitConverter.ToInt16(new byte[] { t2, t1 }, 0);

                    //line += "       " + oc.name + " " + s1.ToString("X4") + s2.ToString("X4");
                    line += "       " + oc.name + " " + i.ToString() + ",(a6)";

                    filePosition += 2;
                }


                else if (oc.name.Contains("BTST"))
                {
                    sbyte s = unchecked((sbyte)fileStuff[filePosition + 3]);
                    line += "       " + oc.name + " " + oc.prefix + s.ToString("X2") + oc.suffix;
                    // Nudge the position counter along by 4
                    filePosition += 4;
                    //sbyte a = unchecked((sbyte)fileStuff[filePosition + 0]);
                    //sbyte b = unchecked((sbyte)fileStuff[filePosition + 1]);
                    //sbyte c = unchecked((sbyte)fileStuff[filePosition + 2]);
                    //sbyte d = unchecked((sbyte)fileStuff[filePosition + 3]);

                    //byte[] bytes = { fileStuff[filePosition + 0], fileStuff[filePosition + 1], fileStuff[filePosition + 2], fileStuff[filePosition + 3] };
                    string n = /*fileStuff[filePosition + 0].ToString("X2") +*/ fileStuff[filePosition + 1].ToString("X2") + fileStuff[filePosition + 2].ToString("X2") + fileStuff[filePosition + 3].ToString("X2");
                    line += n;
                    filePosition += 2;
                }


                // BNE $START + $????
                // If the Opcode Names are branches ...
                else if (oc.name.Contains("BRA") ||
                    oc.name.Contains("BCC") || oc.name.Contains("BCS") ||
                    oc.name.Contains("BEQ") || oc.name.Contains("BMI") ||
                    oc.name.Contains("BNE") || oc.name.Contains("BPL") ||
                    oc.name.Contains("BVC") || oc.name.Contains("BVS"))
                {
                    sbyte s = unchecked((sbyte)fileStuff[filePosition + 1]);
                    s += 2;
                    line += "       " + oc.name + " " + oc.prefix + (pc + s).ToString("X4");
                    filePosition += 2;
                }

                else if (oc.name.Contains("BSR"))
                {
                    byte s = unchecked(fileStuff[filePosition + 2]);
                    byte s1 = unchecked(fileStuff[filePosition + 3]);
                    var i = BitConverter.ToInt16(new byte[] { s1, s }, 0);
                    s += 2;
                    line += "       " + oc.name + " " + oc.prefix + (pc + 2 + i /*s*/).ToString("X4");
                    filePosition += 2;
                }
                else if (oc.name.Contains("MOVEM"))
                {
                    line += "       " + oc.name + " " + "D0-D7/A0-A6,-(A7)";
                    filePosition += 2;
                }

                else
                {
                    line += "       " + oc.name + " " + oc.prefix + fileStuff[filePosition + 1].ToString("X2") + oc.suffix;
                    filePosition += 2;
                }
                filePosition += 2;
            }

            if (oc.numberOfBytes == 4)
            {
                line += " " + fileStuff[filePosition + 2].ToString("X2");

                if (oc.name.Contains("JSR"))
                {
                    sbyte s1 = unchecked((sbyte)fileStuff[filePosition + 3]);
                    sbyte s2 = unchecked((sbyte)fileStuff[filePosition + 4]);
                    sbyte s3 = unchecked((sbyte)fileStuff[filePosition + 5]);
                    line += fileStuff[filePosition + 3].ToString("X2") + fileStuff[filePosition + 4].ToString("X2") + fileStuff[filePosition + 5].ToString("X2") + " " +
                        oc.name + " " + oc.prefix + s1.ToString("X2") + s2.ToString("X2") + s3.ToString("X2");
                    filePosition += 6;
                }

                else if (oc.name.Contains("MOVE"))
                {
                    sbyte s0 = unchecked((sbyte)fileStuff[filePosition + 2]);
                    sbyte s1 = unchecked((sbyte)fileStuff[filePosition + 3]);
                    sbyte s2 = unchecked((sbyte)fileStuff[filePosition + 4]);
                    sbyte s3 = unchecked((sbyte)fileStuff[filePosition + 5]);

                    if (oc.name.Contains("MOVEA")) // 2C79
                    {
                        //line += "       " + oc.name + " " + oc.prefix + fileStuff[filePosition + 3].ToString("X2") + s2.ToString("X2") + s3.ToString("X2") + oc.suffix;
                        line += "       " + oc.name + " " + oc.prefix + s1.ToString("X2") + s2.ToString("X2") + s3.ToString("X2") + oc.suffix;
                    }

                    else
                    {
                        //00031046 317c 8040 0096           MOVE.W #$8040,(A0,$0096) == $00000096 [0840]
                        //line += "       " + oc.name + " " + oc.prefix + fileStuff[filePosition + 3].ToString("X2") + "," + oc.suffix + s2.ToString("X2") + s3.ToString("X2");
                        line += "       " + oc.name + " " + oc.prefix + s0.ToString("X2") + s1.ToString("X2") + oc.suffix + s2.ToString("X2") + s3.ToString("X2") + "(A0)";
                    }

                    filePosition += 6;
                }

                else if (oc.name.Contains("LEA"))
                {
                    sbyte s1 = unchecked((sbyte)fileStuff[filePosition + 3]);
                    sbyte s2 = unchecked((sbyte)fileStuff[filePosition + 4]);
                    sbyte s3 = unchecked((sbyte)fileStuff[filePosition + 5]);
                    line += "       " + oc.name + " " + oc.prefix + fileStuff[filePosition + 3].ToString("X2") + s2.ToString("X2") + s3.ToString("X2") + oc.suffix;
                    filePosition += 6;
                }
                else
                {
                    line += "       " + oc.name + " Not implemented yet";
                    filePosition += 6;
                }
            }

            if (oc.numberOfBytes == 6)
            {
                line += " " + fileStuff[filePosition + 2].ToString("X2");
                if (oc.name.Contains("MOVE")) //A"))
                {
                    //if (oc.name.Contains("MOVE(b)") || oc.name.Contains("MOVE(w)") || oc.name.Contains("MOVE(l)"))
                    //{
                    // 8 bytes ?
                    sbyte s1 = unchecked((sbyte)fileStuff[filePosition + 3]);
                    sbyte s2 = unchecked((sbyte)fileStuff[filePosition + 4]);
                    sbyte s3 = unchecked((sbyte)fileStuff[filePosition + 5]);
                    sbyte s4 = unchecked((sbyte)fileStuff[filePosition + 6]);
                    sbyte s5 = unchecked((sbyte)fileStuff[filePosition + 7]);
                    if (oc.name.Contains("MOVE(l)"))
                    {
                        //0003103E 217c 0003 10c2 0080      MOVE.L #$000310c2,(A0,$0080) == $00000080 [00fc0836]
                        //003E  217C 00       MOVE(l) 03,10C20080
                        line += "       " + oc.name + " " + oc.prefix + fileStuff[filePosition + 3].ToString("X2") + s2.ToString("X2") + s3.ToString("X2") +
                            /*s4.ToString("X2") +*/ oc.suffix + s5.ToString("X2") + "(A0)";
                        //filePosition += 8;// 6;// 2;
                    }
                    if (oc.name.Contains("MOVE(b)"))
                    {
                        //00031006 13fc 0001 0006 c2a0      MOVE.B #$01,$0006c2a0 [00]
                        line += "       " + oc.name + " " + oc.prefix + s1.ToString("X2") + oc.suffix + s2.ToString("X2") + s3.ToString("X2") +
                            s4.ToString("X2") + s5.ToString("X2");
                    }
                    else
                    {
                        ////MOVEA.L $00000004 [00c00276],A6
                        //line += "       " + oc.name + " " + oc.prefix + s1.ToString("X2") + s2.ToString("X2") + s3.ToString("X2") + s4.ToString("X2") + s5.ToString("X2") + oc.suffix;
                        ////line += "       " + oc.name + " " + oc.prefix + fileStuff[filePosition + 3].ToString("X2") + s2.ToString("X2") + s3.ToString("X2") +
                        ////    /*s4.ToString("X2") +*/ "," + oc.suffix + s5.ToString("X2");

                    }
                    filePosition += 8;// 6;// 2;
                }

                else if (oc.name.Contains("CMPI"))
                {
                    line += "       " + oc.name + " " + oc.prefix;
                    line += fileStuff[filePosition + 3].ToString("X2") + oc.suffix;
                    line += fileStuff[filePosition + 5].ToString("X2") + fileStuff[filePosition + 6].ToString("X2") + fileStuff[filePosition + 7].ToString("X2");
                    filePosition += 8;// 6;
                }
                else if (oc.name.Contains("BCHG"))
                {
                    line += "       " + oc.name + " " + oc.prefix;
                    line += fileStuff[filePosition + 3].ToString("X2") + oc.suffix;
                    line += fileStuff[filePosition + 5].ToString("X2") + fileStuff[filePosition + 6].ToString("X2") + fileStuff[filePosition + 7].ToString("X2");
                    filePosition += 8;// 6;
                }
                else if (oc.name.Contains("BSET"))
                {
                    sbyte s = unchecked((sbyte)fileStuff[filePosition + 3]);
                    line += "       " + oc.name + " " + oc.prefix + s.ToString("X2") + oc.suffix;
                    // Nudge the position counter along by 4
                    filePosition += 4;
                    //byte[] bytes = { fileStuff[filePosition + 0], fileStuff[filePosition + 1], fileStuff[filePosition + 2], fileStuff[filePosition + 3] };
                    string n = /*fileStuff[filePosition + 0].ToString("X2") +*/ fileStuff[filePosition + 1].ToString("X2") + fileStuff[filePosition + 2].ToString("X2") + fileStuff[filePosition + 3].ToString("X2");
                    line += n;
                    filePosition += 4;// 2;
                }
                else
                {
                    line += "       " + oc.name + " Not implemented yet";
                    filePosition += 8;
                }
            }



            else if (oc.numberOfBytes == 3 && (filePosition < fileStuff.Length - 3))
            {
                if (oc.illegal)
                {
                    illegalOpCodes.Add(pc.ToString("X4"));
                }

                temp = new string[3];
                temp[0] = "!byte $" + oc.code;
                temp[1] = "!byte $" + fileStuff[filePosition + 1].ToString("X2");
                temp[2] = "!byte $" + fileStuff[filePosition + 2].ToString("X2");
                dataStatements.Add(pc.ToString("X4"), temp);

                line += " " + fileStuff[filePosition + 1].ToString("X2") + " " + fileStuff[filePosition + 2].ToString("X2");
                line += "    " + oc.name + " " + oc.prefix + fileStuff[filePosition + 2].ToString("X2") + fileStuff[filePosition + 1].ToString("X2") + oc.suffix;
                filePosition += 3;
            }
            else if (oc.numberOfBytes == 3 && (filePosition == fileStuff.Length - 2))
            {
                filePosition = fileStuff.Length;
            }
        }

    }
}
