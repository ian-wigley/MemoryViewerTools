﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text;

namespace BinToAssembly
{
    public partial class BinaryConverter : Form
    {
        private readonly string label = "label";
        private readonly string branch = "branch";

        private int labelCount = 0;
        private int branchCount = 0;

        protected List<string> code = new List<string>();
        protected List<string> lineNumbers = new List<string>();

        private readonly List<string> passOne = new List<string>();
        private readonly List<string> passTwo = new List<string>();
        private readonly List<string> passThree = new List<string>();
        private readonly List<string> found = new List<string>();

        private readonly Dictionary<string, string[]> dataStatements = new Dictionary<string, string[]>();
        private readonly Dictionary<string, string> labelLoc = new Dictionary<string, string>();
        private readonly Dictionary<string, string> branchLoc = new Dictionary<string, string>();
        private readonly Dictionary<string, string[]> replacedWithDataCollection = new Dictionary<string, string[]>();

        private readonly PopulateOpCodeList populateOpCodeList = new PopulateOpCodeList();

        private byte[] data;

        public BinaryConverter()
        {
            InitializeComponent();

            // Sets the byte viewer display mode.
            byteviewer.SetDisplayMode(DisplayMode.Hexdump);
            MaximizeBox = false;
            MinimizeBox = false;
            labelGenerator.Enabled = false;
            Compile.Enabled = false;
            generateLabelsToolStripMenuItem.Enabled = false;
            leftWindowToolStripMenuItem.Enabled = false;
            rightWindowToolStripMenuItem.Enabled = false;
            populateOpCodeList.Init();

            Numbers.Font = AssemblyView.Font;
            CompilerTextBox.Cursor = Cursors.Arrow;
            CompilerTextBox.GotFocus += CompilerTextBox_GotFocus;
        }

        /// <summary>
        /// AddLineNumbers
        /// </summary>
        private void AddLineNumbers()
        {
            Point pt = new Point(0, 0);
            int firstIndex = AssemblyView.GetCharIndexFromPosition(pt);
            int firstLine = AssemblyView.GetLineFromCharIndex(firstIndex);
            pt.X = ClientRectangle.Width;
            pt.Y = ClientRectangle.Height;
            int lastIndex = AssemblyView.GetCharIndexFromPosition(pt);
            int lastLine = AssemblyView.GetLineFromCharIndex(lastIndex);
            Numbers.SelectionAlignment = HorizontalAlignment.Center;
            Numbers.Text = "";
            Numbers.Width = GetWidth();
            for (int i = firstLine - 1; i <= lastLine + 1; i++)
            {
                Numbers.Text += i + 1 + "\n";
            }
        }

        /// <summary>
        /// GetWidth
        /// </summary>
        private int GetWidth()
        {
            int line = Numbers.Lines.Length;
            int width;
            if (line <= 99) { width = 20 + (int)Numbers.Font.Size; }
            else if (line <= 999) { width = 30 + (int)Numbers.Font.Size; }
            else { width = 50 + (int)Numbers.Font.Size; }
            return width;
        }

        /// <summary>
        /// Add Labels
        /// </summary>
        private void AddLabels(
            string start,
            string end)
        {
            AssemblyView.Clear();
            ClearRightWindow();
            passThree.Add("                *=$" + start);
            var originalFileContent = code;
            bool firstPass = true;
            int count = 0;

            // First pass parses the content looking for branch & jump conditions
            while (firstPass)
            {
                // Split each line into an array
                // var lineDetails = originalFileContent[count++].Split(' ');
                var lineDetails = textBox1.Lines[count++].Split(' ');

                if (lineDetails.Length > 1)
                {
                    switch (lineDetails[1])
                    {
                        //case "20": // JSR
                        //case "4C": // JMP
                        //    if (!labelLoc.Keys.Contains(lineDetails[4] + lineDetails[3]))
                        //    {
                        //        labelLoc.Add(lineDetails[4] + lineDetails[3], label + labelCount++.ToString());
                        //    }
                        //    passOne.Add(lineDetails[8] + " " + lineDetails[9]);
                        //    break;

                        case "6000": // BRA
                        case "6100": // BSR
                        case "6600": // BNE
                        case "66F2": // BEQ
                        case "670A": // BEQ
                        case "6700":
                            string location = lineDetails[18].Replace("$", "");
                            location = location.Replace("#", "");
                            if (!branchLoc.Keys.Contains(location))
                            {
                                branchLoc.Add(location, branch + branchCount++.ToString());
                            }
                            passOne.Add(lineDetails[17] + " " + lineDetails[18]);
                            break;
                        case "4280": // CLR
                            passOne.Add(lineDetails[21] + " " + lineDetails[22]);
                            break;
                        default:
                            int indexLength = lineDetails.Length;
                            passOne.Add(lineDetails[indexLength - 2] + " " + lineDetails[indexLength - 1]);
                            break;
                    }
                }
                if (count >= int.Parse(end, System.Globalization.NumberStyles.HexNumber) || count >= originalFileContent.Count || lineDetails[0].ToLower().Contains(end.ToLower()))
                {
                    firstPass = false;
                }
            }

            // Second pass iterates through first pass collection adding labels and branches into the code
            int counter = 0;
            for (int i = 0; i < passOne.Count; i++)
            {
                string assembly = passOne[counter++];
                foreach (KeyValuePair<string, string> memLocation in branchLoc)
                {
                    if (originalFileContent[i].ToUpper().Contains(memLocation.Key.ToUpper()))
                    {
                        var dets = assembly.Split(' ');
                        if (dets[0].Contains("BNE") || dets[0].Contains("BEQ") || dets[0].Contains("BSR") || dets[0].Contains("BRA"))
                        {
                            assembly = dets[0] + " " + memLocation.Value;
                        }
                    }
                }
                passTwo.Add(assembly);
            }

            // Add the labels to the front of the code
            counter = 0;
            for (int i = 0; i < passOne.Count; i++)
            {
                var dets = originalFileContent[counter++].Split(' ');
                string label = "                ";
                //foreach (KeyValuePair<string, string> memLocation in labelLoc)
                //{
                //    if (dets[0].ToUpper().Contains(memLocation.Key))
                //    {
                //        label = memLocation.Value + "          ";
                //        // The memory address has been found add it another list
                //        found.Add(memLocation.Key);
                //    }
                //}

                foreach (KeyValuePair<string, string> memLocation in branchLoc)
                {
                    if (dets[0].ToUpper().Contains(memLocation.Key.ToUpper()))
                    {
                        label = memLocation.Value + "         ";
                        found.Add(memLocation.Key);
                    }
                }
                passThree.Add(label + passTwo[i]);
            }

            if (found.Count != branchLoc.Count)
            {
                passThree.Add("\n; ---------------------------------------------------------");
                passThree.Add("; The memory locations below were not found within the file\n");
            }

            // Finally iterate through the found list & add references to the address not found
            foreach (KeyValuePair<string, string> memLocation in branchLoc)
            {
                if (!found.Contains(memLocation.Key))
                {
                    passThree.Add(memLocation.Value + "; @: " + memLocation.Key);
                }
            }

            AssemblyView.Font = new Font(FontFamily.GenericMonospace, AssemblyView.Font.Size);
            AssemblyView.Lines = passThree.ToArray();
            rightWindowToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        ///
        /// </summary>
        private void OpenToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open File",
                InitialDirectory = @"*.*",
                Filter = "All files (*.prg)|*.PRG|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileLoaded.Text = openFileDialog.SafeFileName;
                ClearCollections();
                textBox1.Clear();
                Parser parser = new Parser();
                data = parser.LoadBinaryData(openFileDialog.FileName);
                parser.ParseFileContent(data, populateOpCodeList, textBox1, ref lineNumbers, ref code);
                labelGenerator.Enabled = true;
                byteviewer.SetFile(openFileDialog.FileName);
                generateLabelsToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Method to add labels to the `source code`
        /// </summary>
        public void DisplayMemorySelector()
        {
            char[] startAddress = new char[lineNumbers[0].Length];
            char[] endAddress = new char[lineNumbers[lineNumbers.Count - 1].Length];

            int count = 0;
            foreach (char chr in lineNumbers[0])
            {
                startAddress[count++] = chr;
            }
            count = 0;
            foreach (char chr in lineNumbers[lineNumbers.Count - 1])
            {
                endAddress[count++] = chr;
            }

            MemorySelector ms = new MemorySelector(startAddress, endAddress);
            if (ms.ShowDialog() == DialogResult.OK)
            {
                int start = int.Parse(ms.GetSelectedMemStartLocation, System.Globalization.NumberStyles.HexNumber);
                int end = int.Parse(ms.GetSelectedMemEndLocation, System.Globalization.NumberStyles.HexNumber);

                if (start <= end)
                {
                    AddLabels(ms.GetSelectedMemStartLocation, ms.GetSelectedMemEndLocation);
                }
                else
                {
                    MessageBox.Show("The selected end address exceeds the length of the bytes $" + lineNumbers[lineNumbers.Count - 1]);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void ClearCollections()
        {
            ClearLeftWindow();
            ClearRightWindow();
        }

        /// <summary>
        ///
        /// </summary>
        private void ClearLeftWindow()
        {
            code.Clear();
        }

        /// <summary>
        ///
        /// </summary>
        private void ClearRightWindow()
        {
            AssemblyView.Text = "";
            passOne.Clear();
            passTwo.Clear();
            passThree.Clear();
            found.Clear();
            labelLoc.Clear();
            branchLoc.Clear();
        }

        /// <summary>
        ///
        /// </summary>
        private void LeftWindowToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            Save(code);
        }

        /// <summary>
        ///
        /// </summary>
        private void RightWindowToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            Save(passThree);
        }

        /// <summary>
        ///
        /// </summary>
        private void Save(
            List<string> collection)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save File",
                InitialDirectory = @"*.*",
                Filter = "All files (*.*)|*.*|All files (*.s)|*.s",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog.FileName, collection);
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void GenerateLabelsToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            DisplayMemorySelector();
        }

        /// <summary>
        ///
        /// </summary>
        private void ClearToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            ClearCollections();
            textBox1.Clear();
            AssemblyView.Clear();
            byteviewer.SetBytes(new byte[] { });
            //lineNumbers = 0;
        }

        /// <summary>
        ///
        /// </summary>
        private void LabelGenerator_Click(object sender, EventArgs e)
        {
            // Todo finish implementation
            DisplayMemorySelector();
            Compile.Enabled = true;
            Numbers.Select();
            AddLineNumbers();
        }

        /// <summary>
        ///
        /// </summary>
        private void CompilerTextBox_GotFocus(object sender, EventArgs e)
        {
            ((RichTextBox)sender).Parent.Focus();
        }

        /// <summary>
        ///
        /// </summary>
        private void Compile_Click(object sender, EventArgs e)
        {
            // Make the output visible
            Dissambly.SelectTab(1);
            CompilerTextBox.Text = "";

            // Get a random temporary file name
            string tempFile = Path.GetTempFileName();

            // Convert the lines of Text to a byte array
            byte[] dataAsBytes = AssemblyView.Lines.SelectMany(s => System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();

            // Open a FileStream to write to the file:
            using (Stream fileStream = File.OpenWrite(tempFile))
            {
                fileStream.Write(dataAsBytes, 0, dataAsBytes.Length);
            }

            tempFile = tempFile.Replace("\\", "/");

            var sc = populateOpCodeList.GetXMLLoader.SettingsCache;
            string args = "/C " + sc.VasmLocation +
                " " + sc.Processor +
                " " + sc.Kickhunk +
                " " + sc.Fhunk +
                " " + sc.Flag +
                " " + sc.Destination +
                " " + tempFile;

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = args;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            CompilerTextBox.Text += p.StandardOutput.ReadToEnd();
            CompilerTextBox.Text += p.StandardError.ReadToEnd();

            // Delete the temp file
            File.Delete(tempFile);
        }

        /// <summary>
        ///
        /// </summary>
        private void Configure_Click(object sender, EventArgs e)
        {
            // TODO
        }

        /// <summary>
        ///
        /// </summary>
        private void TextBox2_VScroll(object sender, EventArgs e)
        {
            Numbers.Text = "";
            AddLineNumbers();
            AssemblyView.Invalidate();
        }

        /// <summary>
        /// Convert To Data DCW Click
        /// </summary>
        private void ConvertToDataDCWClick(object sender, EventArgs e)
        {
            // Get any highlighted text
            string selectedText = textBox1.SelectedText;
            string[] splitSelectedText = selectedText.Split('\n');
            if (textBox1.SelectedText != "")
            {
                string str = "";
                foreach (string dataLines in splitSelectedText)
                {
                    int index = dataLines.IndexOf("   ");
                    if (index != -1)
                    {
                        string trimmed = dataLines.Substring(0, index);
                        string[] extraSplit = trimmed.Split(' ');
                        string data = "";
                        str += extraSplit[0] + "                         DC.W ";

                        for (int i = 1; i < extraSplit.Length; i++)
                        {
                            data += "$" + extraSplit[i] + ",";

                        }
                        data = data.Remove(data.LastIndexOf(","));
                        str += data + "\r\n";
                    }
                }
                textBox1.SelectedText = str.Remove(str.LastIndexOf("\r\n"));
                //string[] text = textBox1.Lines;
            }
        }

        /// <summary>
        /// Convert To Data DCB Click
        /// </summary>
        private void ConvertToDataDCBClick(object sender, EventArgs e)
        {
            string selectedText = textBox1.SelectedText;
            string[] splitSelectedText = selectedText.Split('\n');
            var another = splitSelectedText[0].Split(' ');
            int value = Convert.ToInt32(another[0], 16);
            var converted = Encoding.ASCII.GetString(data, value, splitSelectedText.Length);
            string str = another[0] + "                         DC.B " + "'" + converted + "'";
            textBox1.SelectedText = str;
        }
    }
}