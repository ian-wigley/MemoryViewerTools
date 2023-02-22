using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BinToAssembly
{
    public class Parser68000
    {
        private readonly int startAddress = 0;
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
            while (filePosition < data.Length - 4)
            {
                // lookup the first 4 bits of each byte (Nybble)
                int byteOne = data[filePosition] << 8;
                int opCode = byteOne + data[filePosition + 1];

                int lineNumber = startAddress + filePosition;
                lineNumbers.Add(lineNumber.ToString("X4"));
                string line = (startAddress + filePosition).ToString("X4");
                line += "  " + opCode.ToString("X4");
                int pc = startAddress + filePosition;

                bool found = false;
                // Get the Opcode object
                var oc = populateOpCodeList.GetOpCode(opCode.ToString("X4"));

                if (oc != null)
                {
                    ConvertToAssembly(oc, ref line, ref filePosition, data, lineNumber, pc, ref dataStatements, ref illegalOpcodes);
                    found = true;
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
        }

        public void ConvertToAssembly(
            OpCode oc, 
            ref string line, 
            ref int filePosition, 
            byte[] binaryFileData, 
            int? lineNumber,
            int pc, 
            ref Dictionary<string, string[]> 
            dataStatements, ref List<string> 
            illegalOpCodes)
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
                filePosition += 2;
            }
            if (oc.NumberOfBytes == 2)
            {
                if (oc.Illegal)
                {
                    illegalOpCodes.Add(pc.ToString("X4"));
                }
                temp = new string[2] { "!byte $" + oc.Code, "!byte $" + binaryFileData[filePosition + 1].ToString("X2") };
                dataStatements.Add(pc.ToString("X4"), temp);
                line += " " + binaryFileData[filePosition + 2].ToString("X2");

                if (oc.Name.Contains("JSR"))
                {
                    JSR(oc, ref line, ref filePosition, binaryFileData);
                }

                else if (oc.Name.Contains("BTST"))
                {
                    BTST(oc, ref line, ref filePosition, binaryFileData);
                }

                // BNE $START + $????
                // If the Opcode Names are branches ...
                else if (oc.Name.Contains("BRA") ||
                    oc.Name.Contains("BCC") || oc.Name.Contains("BCS") ||
                    oc.Name.Contains("BEQ") || oc.Name.Contains("BMI") ||
                    oc.Name.Contains("BNE") || oc.Name.Contains("BPL") ||
                    oc.Name.Contains("BVC") || oc.Name.Contains("BVS"))
                {
                    sbyte s = unchecked((sbyte)binaryFileData[filePosition + 1]);
                    s += 2;
                    line += "       " + oc.Name + " " + oc.Prefix + (pc + s).ToString("X4");
                    filePosition += 2;
                }

                else if (oc.Name.Contains("BSR"))
                {
                    BSR(oc, ref line, ref filePosition, binaryFileData, pc);
                }
                else if (oc.Name.Contains("MOVEM"))
                {
                    line += "       " + oc.Name + " " + "D0-D7/A0-A6,-(A7)";
                    filePosition += 2;
                }

                else
                {
                    line += "       " + oc.Name + " " + oc.Prefix + binaryFileData[filePosition + 1].ToString("X2") + oc.Suffix;
                    filePosition += 2;
                }
                filePosition += 2;
            }

            if (oc.NumberOfBytes == 4)
            {
                line += " " + binaryFileData[filePosition + 2].ToString("X2");

                if (oc.Name.Contains("JSR"))
                {
                    sbyte s1 = unchecked((sbyte)binaryFileData[filePosition + 3]);
                    sbyte s2 = unchecked((sbyte)binaryFileData[filePosition + 4]);
                    sbyte s3 = unchecked((sbyte)binaryFileData[filePosition + 5]);
                    line += binaryFileData[filePosition + 3].ToString("X2") + binaryFileData[filePosition + 4].ToString("X2") + binaryFileData[filePosition + 5].ToString("X2") + " " +
                        oc.Name + " " + oc.Prefix + s1.ToString("X2") + s2.ToString("X2") + s3.ToString("X2");
                    filePosition += 6;
                }

                else if (oc.Name.Contains("MOVE"))
                {
                    sbyte s0 = unchecked((sbyte)binaryFileData[filePosition + 2]);
                    sbyte s1 = unchecked((sbyte)binaryFileData[filePosition + 3]);
                    sbyte s2 = unchecked((sbyte)binaryFileData[filePosition + 4]);
                    sbyte s3 = unchecked((sbyte)binaryFileData[filePosition + 5]);

                    if (oc.Name.Contains("MOVEA")) // 2C79
                    {
                        //line += "       " + oc.name + " " + oc.prefix + fileStuff[filePosition + 3].ToString("X2") + s2.ToString("X2") + s3.ToString("X2") + oc.suffix;
                        line += "       " + oc.Name + " " + oc.Prefix + s1.ToString("X2") + s2.ToString("X2") + s3.ToString("X2") + oc.Suffix;
                    }

                    else
                    {
                        //00031046 317c 8040 0096           MOVE.W #$8040,(A0,$0096) == $00000096 [0840]
                        //line += "       " + oc.name + " " + oc.prefix + fileStuff[filePosition + 3].ToString("X2") + "," + oc.suffix + s2.ToString("X2") + s3.ToString("X2");
                        line += "       " + oc.Name + " " + oc.Prefix + s0.ToString("X2") + s1.ToString("X2") + oc.Suffix + s2.ToString("X2") + s3.ToString("X2") + "(A0)";
                    }

                    filePosition += 6;
                }

                else if (oc.Name.Contains("LEA"))
                {
                    sbyte s1 = unchecked((sbyte)binaryFileData[filePosition + 3]);
                    sbyte s2 = unchecked((sbyte)binaryFileData[filePosition + 4]);
                    sbyte s3 = unchecked((sbyte)binaryFileData[filePosition + 5]);
                    line += "       " + oc.Name + " " + oc.Prefix + binaryFileData[filePosition + 3].ToString("X2") + s2.ToString("X2") + s3.ToString("X2") + oc.Suffix;
                    filePosition += 6;
                }
                else
                {
                    line += "       " + oc.Name + " Not implemented yet";
                    filePosition += 6;
                }
            }

            if (oc.NumberOfBytes == 6)
            {
                line += " " + binaryFileData[filePosition + 2].ToString("X2");
                if (oc.Name.Contains("MOVE")) //A"))
                {
                    //if (oc.name.Contains("MOVE(b)") || oc.name.Contains("MOVE(w)") || oc.name.Contains("MOVE(l)"))
                    //{
                    // 8 bytes ?
                    sbyte s1 = unchecked((sbyte)binaryFileData[filePosition + 3]);
                    sbyte s2 = unchecked((sbyte)binaryFileData[filePosition + 4]);
                    sbyte s3 = unchecked((sbyte)binaryFileData[filePosition + 5]);
                    sbyte s4 = unchecked((sbyte)binaryFileData[filePosition + 6]);
                    sbyte s5 = unchecked((sbyte)binaryFileData[filePosition + 7]);
                    if (oc.Name.Contains("MOVE(l)"))
                    {
                        //0003103E 217c 0003 10c2 0080      MOVE.L #$000310c2,(A0,$0080) == $00000080 [00fc0836]
                        //003E  217C 00       MOVE(l) 03,10C20080
                        line += "       " + oc.Name + " " + oc.Prefix + binaryFileData[filePosition + 3].ToString("X2") + s2.ToString("X2") + s3.ToString("X2") +
                            /*s4.ToString("X2") +*/ oc.Suffix + s5.ToString("X2") + "(A0)";
                        //filePosition += 8;// 6;// 2;
                    }
                    if (oc.Name.Contains("MOVE(b)"))
                    {
                        //00031006 13fc 0001 0006 c2a0      MOVE.B #$01,$0006c2a0 [00]
                        line += "       " + oc.Name + " " + oc.Prefix + s1.ToString("X2") + oc.Suffix + s2.ToString("X2") + s3.ToString("X2") +
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

                else if (oc.Name.Contains("CMPI"))
                {
                    line += "       " + oc.Name + " " + oc.Prefix;
                    line += binaryFileData[filePosition + 3].ToString("X2") + oc.Suffix;
                    line += binaryFileData[filePosition + 5].ToString("X2") + binaryFileData[filePosition + 6].ToString("X2") + binaryFileData[filePosition + 7].ToString("X2");
                    filePosition += 8;// 6;
                }
                else if (oc.Name.Contains("BCHG"))
                {
                    line += "       " + oc.Name + " " + oc.Prefix;
                    line += binaryFileData[filePosition + 3].ToString("X2") + oc.Suffix;
                    line += binaryFileData[filePosition + 5].ToString("X2") + binaryFileData[filePosition + 6].ToString("X2") + binaryFileData[filePosition + 7].ToString("X2");
                    filePosition += 8;// 6;
                }
                else if (oc.Name.Contains("BSET"))
                {
                    sbyte s = unchecked((sbyte)binaryFileData[filePosition + 3]);
                    line += "       " + oc.Name + " " + oc.Prefix + s.ToString("X2") + oc.Suffix;
                    // Nudge the position counter along by 4
                    filePosition += 4;
                    //byte[] bytes = { fileStuff[filePosition + 0], fileStuff[filePosition + 1], fileStuff[filePosition + 2], fileStuff[filePosition + 3] };
                    string n = /*fileStuff[filePosition + 0].ToString("X2") +*/ binaryFileData[filePosition + 1].ToString("X2") + binaryFileData[filePosition + 2].ToString("X2") + binaryFileData[filePosition + 3].ToString("X2");
                    line += n;
                    filePosition += 4;// 2;
                }
                else
                {
                    line += "       " + oc.Name + " Not implemented yet";
                    filePosition += 8;
                }
            }

            else if (oc.NumberOfBytes == 3 && (filePosition < binaryFileData.Length - 3))
            {
                if (oc.Illegal)
                {
                    illegalOpCodes.Add(pc.ToString("X4"));
                }

                temp = new string[3];
                temp[0] = "!byte $" + oc.Code;
                temp[1] = "!byte $" + binaryFileData[filePosition + 1].ToString("X2");
                temp[2] = "!byte $" + binaryFileData[filePosition + 2].ToString("X2");
                dataStatements.Add(pc.ToString("X4"), temp);

                line += " " + binaryFileData[filePosition + 1].ToString("X2") + " " + binaryFileData[filePosition + 2].ToString("X2");
                line += "    " + oc.Name + " " + oc.Prefix + binaryFileData[filePosition + 2].ToString("X2") + binaryFileData[filePosition + 1].ToString("X2") + oc.Suffix;
                filePosition += 3;
            }
            else if (oc.NumberOfBytes == 3 && (filePosition == binaryFileData.Length - 2))
            {
                filePosition = binaryFileData.Length;
            }
        }

        private void BSR(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData, int pc)
        {
            byte s = unchecked(binaryFileData[filePosition + 2]);
            byte s1 = unchecked(binaryFileData[filePosition + 3]);
            var i = BitConverter.ToInt16(new byte[] { s1, s }, 0);
            line += "       " + oc.Name + " " + oc.Prefix + (pc + 2 + i /*s*/).ToString("X4");
            filePosition += 2;
        }

        private void BTST(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            sbyte s = unchecked((sbyte)binaryFileData[filePosition + 3]);
            line += "       " + oc.Name + " " + oc.Prefix + s.ToString("X2") + oc.Suffix;
            // Nudge the position counter along by 4
            filePosition += 4;
            line += binaryFileData[filePosition + 1].ToString("X2") + binaryFileData[filePosition + 2].ToString("X2") + binaryFileData[filePosition + 3].ToString("X2");
            filePosition += 2;
        }

        private void JSR(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            byte t1 = unchecked(binaryFileData[filePosition + 2]);
            byte t2 = unchecked(binaryFileData[filePosition + 3]);
            short i = BitConverter.ToInt16(new byte[] { t2, t1 }, 0);
            line += "       " + oc.Name + " " + i.ToString() + ",(a6)";
            filePosition += 2;
        }
    }
}
