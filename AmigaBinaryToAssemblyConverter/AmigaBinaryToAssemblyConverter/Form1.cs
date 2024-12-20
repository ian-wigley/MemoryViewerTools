using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace BinToAssembly
{
    public partial class BinaryConverter : Form
    {
        private readonly string label = "label";
        private readonly string branch = "branch";
        private int labelCount = 0;
        private int branchCount = 0;
        private int startAddress = 0;

        private List<string> code = new List<string>();
        private List<string> passOne = new List<string>();
        private List<string> passTwo = new List<string>();
        private List<string> passThree = new List<string>();
        private List<string> found = new List<string>();
        private List<string> lineNumbers = new List<string>();
        private List<string> illegalOpcodes = new List<string>();

        private Dictionary<string, string[]> dataStatements = new Dictionary<string, string[]>();
        private Dictionary<string, string> labelLoc = new Dictionary<string, string>();
        private Dictionary<string, string> branchLoc = new Dictionary<string, string>();

        private readonly PopulateOpCodeList populateOpCodeList = new PopulateOpCodeList();

        private const string m68000 = "68000";

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
            comboBox1.Items.Insert(0, m68000);
            comboBox1.SelectedIndex = 0;
            populateOpCodeList.Init(comboBox1.Items[0].ToString());

            Numbers.Font = AssemblyView.Font;
            CompilerTextBox.Cursor = Cursors.Arrow;
            CompilerTextBox.GotFocus += CompilerTextBox_GotFocus;
        }

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

        private int GetWidth()
        {
            int line = Numbers.Lines.Length;
            int width;
            if (line <= 99)
            {
                width = 20 + (int)Numbers.Font.Size;
            }
            else if (line <= 999)
            {
                width = 30 + (int)Numbers.Font.Size;
            }
            else
            {
                width = 50 + (int)Numbers.Font.Size;
            }
            return width;
        }

        private void AddLabels(
            string start,
            string end,
            bool replaceIllegalOpcodes,
            Dictionary<string, string[]> bucket,
            int firstOccurrence,
            int lastOccurrence)
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
                var lineDetails = originalFileContent[count++].Split(' ');

                if (lineDetails.Length > 1)
                {
                    // Replace the Illegal Opcodes with data statement
                    if (replaceIllegalOpcodes && bucket.TryGetValue(lineDetails[0], out string[] dataValue))
                    {
                        foreach (string str in dataValue)
                        {
                            passOne.Add(str);
                        }
                    }
                    else
                    {
                        switch (lineDetails[1])
                        {
                            case "20": // JSR
                            case "4C": // JMP
                                if (!labelLoc.Keys.Contains(lineDetails[4] + lineDetails[3]))
                                {
                                    labelLoc.Add(lineDetails[4] + lineDetails[3], label + labelCount++.ToString());
                                }
                                passOne.Add(lineDetails[8] + " " + lineDetails[9]);
                                break;
                            case "90": // BCC
                            case "B0": // BCS
                            case "F0": // BEQ
                            case "30": // BMI
                            case "6600": // BNE
                            case "66F2":
                            case "10": // BPL
                            case "50": // BVC
                            case "70": // BVS
                                if (!branchLoc.Keys.Contains(lineDetails[18].Replace("$", "")))
                                {
                                    branchLoc.Add(lineDetails[18].Replace("#$", ""), branch + branchCount++.ToString());
                                }
                                passOne.Add(lineDetails[17] + " " + lineDetails[18]);
                                break;
                            default:
                                int indexLength = lineDetails.Length;
                                passOne.Add(lineDetails[indexLength - 2] + " " + lineDetails[indexLength - 1]);
                                break;
                        }
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

                string label = "";
                string assembly = passOne[counter++];
                foreach (KeyValuePair<String, String> memLocation in labelLoc)
                {
                    if (passOne[i].ToUpper().Contains(memLocation.Key))
                    //   if (originalFileContent[i].ToUpper().Contains(memLocation.Key))
                    {
                        var dets = assembly.Split(' ');
                        if (dets[0].Contains("JSR") || dets[0].Contains("JMP"))
                        {
                            assembly = dets[0] + " " + memLocation.Value;
                        }
                    }
                }
                foreach (KeyValuePair<String, String> memLocation in branchLoc)
                {
                    if (originalFileContent[i].ToUpper().Contains(memLocation.Key.ToUpper()))
                    {
                        var dets = assembly.Split(' ');
                        if (dets[0].Contains("BNE") || dets[0].Contains("BEQ") || dets[0].Contains("BPL"))
                        {
                            assembly = dets[0] + " " + memLocation.Value;
                        }
                    }
                }
                passTwo.Add(label + assembly);
            }

            // Add the labels to the front of the code
            counter = 0;
            for (int i = 0; i < passOne.Count; i++)
            {
                var dets = originalFileContent[counter++].Split(' ');
                string label = "                ";
                foreach (KeyValuePair<String, String> memLocation in labelLoc)
                {
                    if (dets[0].ToUpper().Contains(memLocation.Key))
                    {
                        label = memLocation.Value + "          ";
                        // The memory address has been found add it another list
                        found.Add(memLocation.Key);
                    }
                }

                foreach (KeyValuePair<String, String> memLocation in branchLoc)
                {
                    if (dets[0].ToUpper().Contains(memLocation.Key.ToUpper()))
                    {
                        label = memLocation.Value + "         ";
                    }
                }
                passThree.Add(label + passTwo[i]);
            }

            // Finally iterate through the found list & add references to the address not found
            foreach (KeyValuePair<String, String> memLocation in labelLoc)
            {
                if (!found.Contains(memLocation.Key))
                {
                    passThree.Add(memLocation.Value + " = $" + memLocation.Key);
                }
            }

            AssemblyView.Font = new Font(FontFamily.GenericMonospace, AssemblyView.Font.Size);
            AssemblyView.Lines = passThree.ToArray();
            rightWindowToolStripMenuItem.Enabled = true;
        }

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
                ClearCollections();
                textBox1.Clear();
                Parser68000 p68000 = new Parser68000();
                var data = p68000.LoadBinaryData(openFileDialog.FileName);
                p68000.ParseFileContent(data, populateOpCodeList, textBox1, ref lineNumbers, ref code);
                labelGenerator.Enabled = true;
                byteviewer.SetFile(openFileDialog.FileName);
                generateLabelsToolStripMenuItem.Enabled = true;
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void GenerateLabels()
        {
            char[] startAddress = new char[lineNumbers[0].Length];
            char[] endAddress = new char[lineNumbers[lineNumbers.Count - 1].Length];
            int firstOccurrence = 0;
            int lastOccurrence = 0;

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
                bool firstIllegalOpcodeFound = false;
                Dictionary<string, string[]> replacedWithDataCollection = new Dictionary<string, string[]>();

                if (start <= end)
                {
                    //Check to see if illegal opcodes exist within the code selection
                    for (int i = start; i < end; i++)
                    {
                        if (illegalOpcodes.Contains(i.ToString("X4")))
                        {
                            if (i > firstOccurrence & !firstIllegalOpcodeFound)
                            {
                                firstOccurrence = i;
                                firstIllegalOpcodeFound = true;
                            }
                            if (i > lastOccurrence)
                            {
                                lastOccurrence = i;
                            }
                        }
                    }

                    var temp = lastOccurrence.ToString("X4");
                    int index = 0;
                    foreach (string str in code)
                    {
                        if (str.Contains(temp))
                        {
                            // nudge the last Occurrence along to the next valid opCode
                            //lastOccurrence = int.Parse(lineNumbers[++index], System.Globalization.NumberStyles.HexNumber);
                        }
                        index++;
                    }

                    for (int i = firstOccurrence; i < lastOccurrence; i++)
                    {
                        // Replace the Illegal Opcodes with data statement
                        if (dataStatements.TryGetValue(i.ToString("X4"), out string[] dataValue))
                        {
                            replacedWithDataCollection.Add(i.ToString("X4"), dataValue);
                        }
                    }

                    DialogResult result = DialogResult.Yes;
                    if (firstIllegalOpcodeFound)
                    {
                        result = MessageBox.Show("Illegal Opcodes found within the selection from : " + firstOccurrence.ToString("X4") + " to " + lastOccurrence.ToString("X4") + "\n"
                        + "Replace Illegal Opcodes with data statements ?", " ", MessageBoxButtons.YesNo);
                    }

                    bool convertToBytes = false;
                    if (result == DialogResult.Yes)
                    {
                        convertToBytes = true;
                    }
                    AddLabels(ms.GetSelectedMemStartLocation,
                        ms.GetSelectedMemEndLocation,
                        convertToBytes,
                        replacedWithDataCollection,
                        firstOccurrence,
                        lastOccurrence);
                }
                else
                {
                    MessageBox.Show("The selected end address exceeds the length of the bytes $" + lineNumbers[lineNumbers.Count - 1]);
                }
            }
        }

        private void ClearCollections()
        {
            ClearLeftWindow();
            ClearRightWindow();
        }

        private void ClearLeftWindow()
        {
            code.Clear();
        }

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

        private void LeftWindowToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            Save(code);
        }

        private void RightWindowToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            Save(passThree);
        }

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

        private void GenerateLabelsToolStripMenuItem_Click(
            object sender,
            EventArgs e)
        {
            GenerateLabels();
        }

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

        private void OpenContextMenuItem_Click(
            object sender,
            EventArgs e)
        {
            // Get any highlighted text
            string selectedText = this.textBox1.SelectedText;
            string[] split = selectedText.Split('\n');
            string[] text = this.textBox1.Lines;
            this.textBox1.SelectedText = this.textBox1.SelectedText.Replace(split[0], "DC.W $0FFF");
            // Todo finish implementation
        }

        private void LabelGenerator_Click(object sender, EventArgs e)
        {
            // Todo finish implementation
            GenerateLabels();
            Compile.Enabled = true;
            Numbers.Select();
            AddLineNumbers();
        }

        private void CompilerTextBox_GotFocus(object sender, EventArgs e)
        {
            ((RichTextBox)sender).Parent.Focus();
        }


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

        private void Configure_Click(object sender, EventArgs e)
        {

        }

        private void TextBox2_VScroll(object sender, EventArgs e)
        {
            Numbers.Text = "";
            AddLineNumbers();
            AssemblyView.Invalidate();
        }
    }
}