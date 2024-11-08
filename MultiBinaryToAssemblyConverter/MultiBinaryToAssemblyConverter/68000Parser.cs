﻿using System;
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
            switch (oc.Code)
            {
                case "51C8":
                    DBF(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "4E75":
                    RTS(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "C3FC":
                    MULS(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "0600":
                case "5240":
                case "D5C1":
                    ADD(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "4A11":
                case "4A13":
                    TST(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "6000":
                    BRA(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "41":
                case "42":
                case "43":
                case "44":
                case "46":
                case "47":
                case "1200":
                case "1211":
                case "1219":
                case "1281":
                case "12E9":
                case "13D1":
                case "13FC":
                case "317C":
                case "217C":
                case "2279":
                case "23C9":
                case "289A":
                case "296A":
                case "2C78":
                case "2C79":
                case "303C":
                case "33D2":
                case "3412":
                case "3482":
                case "34EA":
                case "48E7":
                case "4CDF":
                    MOVE(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "41F9":
                case "43F9":
                case "45F9":
                case "47F9":
                case "49EC":
                case "49F9":
                    LEA(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "4268":
                case "42A8":
                case "4280":
                case "4281":
                    CLR(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "0828":
                case "0839":
                    BTST(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "6700":
                    BEQ(oc, ref line, ref filePosition, binaryFileData);
                    break;


                case "6600":
                case "66F6":
                    BNE(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "0401":
                case "0439":
                    SUB(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "0C11":
                case "B21B":
                    CMP(oc, ref line, ref filePosition, binaryFileData);
                    break;

                case "6100":
                    BSR(oc, ref line, ref filePosition, binaryFileData);
                    break;

                default:
                    filePosition = binaryFileData.Length;
                    break;
            }

        }



        private void DBF(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }

        private void RTS(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }

        private void MULS(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }


        private void ADD(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }


        private void TST(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }


        private void BRA(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }

        private void BEQ(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }

        private void BNE(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }

        private void BSR(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }


        private void BTST(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }

        private void CLR(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }

        private void CMP(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }


        private void LEA(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }

        private void MOVE(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }

        private void SUB(OpCode oc, ref string line, ref int filePosition, byte[] binaryFileData)
        {
            line += oc.Detail(ref filePosition, binaryFileData);
        }

    }
}
