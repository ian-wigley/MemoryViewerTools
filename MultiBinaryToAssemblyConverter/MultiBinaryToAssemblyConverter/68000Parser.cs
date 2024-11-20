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
        private Dictionary<string, string[]> dataStatements = new Dictionary<string, string[]>();

        public byte[] LoadBinaryData(string fileName)
        {
            try
            {
                return File.ReadAllBytes(fileName);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occured whilst loading data ", exception.Message);
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
                    ConvertToAssembly(oc, ref line, ref filePosition, data, lineNumber, pc, ref dataStatements);
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
            ref Dictionary<string, string[]> dataStatements 
            )
        {
            switch (oc.Code)
            {
                case "0000": // OR
                case "0001":
                case "0002":
                case "0079":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "66CA": // BNE
                case "66F2":
                case "6772":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "6008": // BRA
                case "60EC":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break; 
                case "51C8": // DBF
                case "51CA":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "4E75": // RTS
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "C3FC":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "0600": // ADD
                case "0611":
                case "5239":
                case "5240":
                case "5243":
                case "5261":
                case "544B":
                case "5C4B":
                case "D27C":
                case "D5C1":
                case "D67C":
                case "DE46":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "4A11":
                case "4A13":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "6000":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "41": // MOVE
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
                case "1E3C":
                case "13D1":
                case "13FC":
                case "1683":
                case "203C":
                case "207A":
                case "20B8":
                case "2239":
                case "223C":
                case "2240":
                case "23C0":
                case "217C":
                case "2279":
                case "23C8":
                case "23C9":
                case "267A":
                case "2829":
                case "289A":
                case "296A":
                case "2C78":
                case "2C79":
                case "2D7A":
                case "303C":
                case "30C1":
                case "30C3":
                case "30FC":
                case "317C":
                case "323C":
                case "33D2":
                case "33EE":
                case "3412":
                case "3607":
                case "363C":
                case "3482":
                case "34EA":
                case "3C3C":
                case "3D7A":
                case "3D7C":
                case "48E7":
                case "4CDF":
                case "7000":
                case "7450":
                case "2E6C":
                case "7261":
                case "7279":
                case "7665":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "41F9":
                case "43F9":
                case "43FA":
                case "45F9":
                case "47F9":
                case "49EC":
                case "49F9":
                case "4DF9":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "4240": // CLR
                case "4268":
                case "4278":
                case "4280":
                case "4281":
                case "42A8":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "0828":
                case "0839":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "6700":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "6600":
                case "66F6":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "0401": // SUB
                case "0439":
                case "5761":
                case "927C":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "0C11": // CMP
                case "B21B":
                case "B240":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "6100": // BSR
                case "6170":
                case "6869":
                case "6373":
                case "6974":
                case "6D6F": // BGE
                case "6C6F":
                case "6F70":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "4EAE":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "4446":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "4E71": // NOP
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "E289": // LSR
                case "EE49":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "0033": // ILLEGAL
                case "494E":
                case "7331":
                case "7332":
                case "7374":
                case "7365":
                case "7761":
                case "FFFF":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                default:
                    filePosition = binaryFileData.Length;
                    break;
            }
        }
    }
}
