using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text;

namespace BinToAssembly
{
    public class Parser
    {
        private readonly int startAddress = 0;
        private Dictionary<string, string[]> dataStatements = new Dictionary<string, string[]>();
        
        //gfxlib:     dc.b    "graphics.library",0,0
        private string graphicsLibrary = "graphics.library";

        /// <summary>
        ///
        /// </summary>
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

        /// <summary>
        ///
        /// </summary>
        public void ParseFileContent(
            byte[] data,
            PopulateOpCodeList populateOpCodeList,
            TextBox textBox,
            ref List<string> lineNumbers,
            ref List<string> code
            )
        {
            int start = -10;
            string converted = Encoding.UTF8.GetString(data, 0, data.Length);
            if (converted.Contains(graphicsLibrary))
            {
                DialogResult result = MessageBox.Show("The text `graphics.library` found do\n you want to convert this to data?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    start = converted.IndexOf(graphicsLibrary);
                }
            }
            textBox.Clear();
            int filePosition = 0;
            while (filePosition < data.Length - 4)
            {
                int firstByte = data[filePosition];
                int secondByte = data[filePosition + 1];

                // lookup the first 4 bits of each byte (Nybble)
                int byteOne = data[filePosition] << 8;
                int opCode = byteOne + data[filePosition + 1];
                int lineNumber = startAddress + filePosition;
                lineNumbers.Add(lineNumber.ToString("X8"));
                string line = (startAddress + filePosition).ToString("X8");
                line += " " + opCode.ToString("X4");
                int pc = startAddress + filePosition;
                bool found = false;

                // Get the Opcode object
                var oc = populateOpCodeList.GetOpCode(firstByte.ToString("X2"), secondByte.ToString("X2"));

                if (filePosition == start || filePosition == start + 1)
                {
                    filePosition = filePosition % 2 == 0 ? filePosition : filePosition + 1;
                    filePosition = ConvertDataToByte(filePosition, out line, out oc);
                }

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

        /// <summary>
        ///
        /// </summary>
        private int ConvertDataToByte(int filePosition, out string line, out dynamic oc)
        {
            line = (startAddress + filePosition).ToString("X8") + "                         DC.B '" + graphicsLibrary + "'";
            oc = null;
            filePosition += graphicsLibrary.Length;
            return filePosition;
        }

        /// <summary>
        ///
        /// </summary>
        private void ConvertDataToWords()
        {
            // TODO
        }

        /// <summary>
        ///
        /// </summary>
        public void ConvertToAssembly(
            dynamic oc,
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
                case "0003":
                case "0004":
                case "0014":
                case "0015":
                case "0016":
                case "006F":
                case "0079":
                case "00B1":
                case "00B3":
                case "00B4":
                case "00B5":
                case "00B6":
                case "00B7":
                case "00B8":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "6600": // BNE
                case "66CA":
                case "66CE":
                case "66F2":
                case "66F6":
                case "670A": // BEQ
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
                case "5480":
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
                case "0A00": // EOR
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "E218": // ROR
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "0041": // MOVE
                case "0042":
                case "0043":
                case "0044":
                case "0046":
                case "0047":
                case "1039":
                case "1200":
                case "1211":
                case "1219":
                case "1281":
                case "12E9":
                case "1E3C":
                case "13D1":
                case "13FC":
                case "1683":
                case "2039":
                case "2079":
                case "203C":
                case "207A":
                case "20B8":
                case "2239":
                case "223C":
                case "2240":
                case "23C0":
                case "217C":
                case "2209":
                case "2279":
                case "227A":
                case "23C8":
                case "23C9":
                case "23E9":
                case "23FC":
                case "267A":
                case "2829":
                case "289A":
                case "296A":
                case "2B48":
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
                case "33F0":
                case "33FC":
                case "3412":
                case "3481":
                case "3607":
                case "3613":
                case "363C":
                case "3681":
                case "3482":
                case "34EA":
                case "3B7C":
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
                case "41F9": // LEA
                case "43F9":
                case "43FA":
                case "45F9":
                case "47F9":
                case "49EC":
                case "49F9":
                case "4BF9":
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
                case "0401": // SUB
                case "0439":
                case "5761":
                case "927C":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "0C00": // CMP
                case "0C2D":
                case "0C40":
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
                case "4841": // SWAP
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "0008":
                case "0033": // ILLEGAL
                case "00E0":
                case "0888":
                case "0999":
                case "0BBB":
                case "0CCC":
                case "0FFF":
                case "4143":
                case "436F":
                case "444D":
                case "4F4E":
                case "494E":
                case "7331":
                case "7332":
                case "735C":
                case "7374":
                case "7365":
                case "7565":
                case "7761":
                case "FFFF":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;

                case "6572": // To sort & put into order ...
                case "0111":
                case "01B0":
                case "0DDD":
                case "0EEE":
                case "6261":
                case "7232":
                case "696E":
                case "7265":
                case "7463":
                case "6F75":
                case "6E74":
                case "0146":
                case "6578":
                case "5445":
                case "4E41":
                case "5253":
                case "6176":
                case "6500":
                case "6368":
                case "6563":
                case "6B6D":
                case "7200":
                case "636F":
                case "7070":
                case "6C69":
                case "4C49":
                case "4E45":
                case "633A":
                case "5C55":
                case "7273":
                case "5C69":
                case "616E":
                case "5C44":
                case "6F77":
                case "6E6C":
                case "6F61":
                case "6473":
                case "5C76":
                case "7363":
                case "6F64":
                case "652D":
                case "616D":
                case "6967":
                case "612D":
                case "776B":
                case "732D":
                case "706C":
                case "7769":
                case "6E64":
                case "735F":
                case "7836":
                case "345C":
                case "7673":
                case "6465":
                case "2D61":
                case "6D69":
                case "6761":
                case "2D77":
                case "6B73":
                case "2D65":
                case "7861":
                case "6D70":
                case "6C65":
                case "5C67":
                case "656E":
                case "702E":
                case "7300":
                case "0176":
                case "01C0":
                case "01D2":
                case "01D4":
                case "01D8":
                case "01E2":
                case "6766":
                case "786E":
                case "5361":
                case "6E63":
                case "6C75":
                case "6963":
                case "7068":
                case "5F6C":
                case "2E69":
                case "0102": // BTST
                case "0106":
                case "0777":
                case "0333":
                case "0AAA":
                case "0444":
                case "0222":
                case "0666":
                case "5000":
                case "0108":
                case "010A":
                case "0034":
                case "01FC":
                case "0555":
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "4EB9":  // jsr
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                case "48F9": // movem
                    line += oc.Detail(ref filePosition, binaryFileData);
                    break;
                default:
                    filePosition = binaryFileData.Length;
                    break;
            }
        }
    }
}
