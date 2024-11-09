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

        public byte[] LoadBinaryData(string fileName)
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
                    //ConvertToAssembly(oc, ref line, ref filePosition, data, lineNumber, pc, ref dataStatements, ref illegalOpcodes);
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
            //ref List<string> illegalOpCodes
            )
        {
            switch (oc.Code)
            {
                case "51C8":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                case "4E75":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                case "C3FC":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                case "0600":
                case "5240":
                case "D5C1":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                case "4A11":
                case "4A13":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                case "6000":
                    line += oc.Detail(ref filePosition, binaryFileData);
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
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                case "41F9":
                case "43F9":
                case "45F9":
                case "47F9":
                case "49EC":
                case "49F9":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                case "4268":
                case "42A8":
                case "4280":
                case "4281":
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

                case "0401":
                case "0439":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                case "0C11":
                case "B21B":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                case "6100":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                default:
                    filePosition = binaryFileData.Length;
                    break;
            }
        }
    }
}
