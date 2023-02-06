using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace BinToAssembly
{
    public partial class Form1 : Form
    {
        private string label = "label";
        private string branch = "branch";
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

        private PopulateOpCodeList populateOpCodeList = new PopulateOpCodeList();

        private ByteViewer byteviewer;

        private const string m6502 = "6502";
        private const string m68xx = "68xx";
        private const string m68000 = "68000";

        public Form1()
        {
            InitializeComponent();

            // Sets the byteviewer display mode.
            byteviewer.SetDisplayMode(DisplayMode.Hexdump);

            MaximizeBox = false;
            MinimizeBox = false;
            generateLabelsToolStripMenuItem.Enabled = false;
            leftWindowToolStripMenuItem.Enabled = false;
            rightWindowToolStripMenuItem.Enabled = false;
            comboBox1.Items.Insert(0, m68000);
            //comboBox1.Items.Insert(0, m6502);
            //comboBox1.Items.Insert(1, m68xx);
            //comboBox1.Items.Insert(2, m68000);
            comboBox1.SelectedIndex = 0;

            populateOpCodeList.Init(comboBox1.Items[0].ToString());
        }

        private void AddLabels(string start, string end, bool replaceIllegalOpcodes, Dictionary<string, string[]> bucket, int firstOccurance, int lastOccurrance)
        {
            textBox2.Clear();
            ClearRightWindow();
            passThree.Add("                *=$" + start);
            var originalFileContent = code;
            bool firstPass = true;
            int count = 0;

            // First pass parses the content looking for branch & jump conditions
            while (firstPass)
            {
                //Split each line into an array
                var lineDetails = originalFileContent[count++].Split(' ');

                if (lineDetails.Length > 1)
                {
                    string[] dataValue;
                    // Replace the Illegal Opcodes with data statement
                    if (replaceIllegalOpcodes && bucket.TryGetValue(lineDetails[0], out dataValue))
                    {
                        foreach (string str in dataValue)
                        {
                            passOne.Add(str);
                        }
                    }
                    else
                    {
                        switch (lineDetails[2].ToUpper())
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
                            case "D0": // BNE
                            case "10": // BPL 
                            case "50": // BVC
                            case "70": // BVS
                                if (!branchLoc.Keys.Contains(lineDetails[11].Replace("$", "")))
                                {
                                    branchLoc.Add(lineDetails[11].Replace("$", ""), branch + branchCount++.ToString());
                                }
                                passOne.Add(lineDetails[10] + " " + lineDetails[11]);
                                break;
                            default:
                                if (lineDetails[3] == "" && lineDetails[4] == "")
                                {
                                    passOne.Add(lineDetails[12]);
                                }
                                else if (lineDetails[3] != "" && lineDetails[4] == "")
                                {
                                    passOne.Add(lineDetails[10] + " " + lineDetails[11]);
                                }
                                else if (lineDetails[3] != "" && lineDetails[4] != "")
                                {
                                    passOne.Add(lineDetails[8] + " " + lineDetails[9]);
                                }
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
                    if (originalFileContent[i].ToUpper().Contains(memLocation.Key))
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
                        // The moemory address has been found add it another list
                        found.Add(memLocation.Key);
                    }
                }

                foreach (KeyValuePair<String, String> memLocation in branchLoc)
                {
                    if (dets[0].ToUpper().Contains(memLocation.Key))
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

            textBox2.Font = new Font(FontFamily.GenericMonospace, textBox2.Font.Size);
            textBox2.Lines = passThree.ToArray();
            rightWindowToolStripMenuItem.Enabled = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
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
                // Check what set of Opcodes are loaded
                if (populateOpCodeList.GetProcessor != comboBox1.Text)
                {//.SelectedText) {
                    populateOpCodeList.Init(comboBox1.SelectedText);
                }

                ClearCollections();
                textBox1.Clear();
                MemoryLocation ml = new MemoryLocation();
                if (ml.ShowDialog() == DialogResult.OK)
                {
                    int.TryParse(ml.GetMemStartLocation, out startAddress);
                    //                   ParseFileContent(openFileDialog.FileName);
                    if (comboBox1.Text.Equals(m6502))
                    {
                        Parser6502 p6502 = new Parser6502();
                        p6502.ParseFileContent(openFileDialog.FileName, populateOpCodeList, textBox1, ref lineNumbers, ref code);
                    }
                    if (comboBox1.Text.Equals(m68xx))
                    {
                        Parser68xx p68xx = new Parser68xx();
                        p68xx.ParseFileContent(openFileDialog.FileName, populateOpCodeList, textBox1);
                    }
                    if (comboBox1.Text.Equals(m68000))
                    {
                        Parser68000 p68000 = new Parser68000();
                        var data = p68000.LoadData(openFileDialog.FileName);
                        p68000.ParseFileContent(data, populateOpCodeList, textBox1, ref lineNumbers, ref code);
                    }
                }

                byteviewer.SetFile(openFileDialog.FileName);
                generateLabelsToolStripMenuItem.Enabled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void generateLabels()
        {
            char[] startAdress = new char[lineNumbers[0].Length];
            char[] endAdress = new char[lineNumbers[lineNumbers.Count - 1].Length];
            int firstOccurance = 0;
            int lastOccurrance = 0;

            int count = 0;
            foreach (char chr in lineNumbers[0])
            {
                startAdress[count++] = chr;
            }
            count = 0;
            foreach (char chr in lineNumbers[lineNumbers.Count - 1])
            {
                endAdress[count++] = chr;
            }

            MemorySelector ms = new MemorySelector(startAdress, endAdress);
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
                            if (i > firstOccurance & !firstIllegalOpcodeFound)
                            {
                                firstOccurance = i;
                                firstIllegalOpcodeFound = true;
                            }
                            if (i > lastOccurrance)
                            {
                                lastOccurrance = i;
                            }
                        }
                    }

                    var temp = lastOccurrance.ToString("X4");
                    int index = 0;
                    foreach (string str in code)
                    {
                        if (str.Contains(temp))
                        {
                            // nudge the last Occurance along to the next valid opCode
                            lastOccurrance = int.Parse(lineNumbers[++index], System.Globalization.NumberStyles.HexNumber);
                        }
                        index++;
                    }

                    for (int i = firstOccurance; i < lastOccurrance; i++)
                    {
                        string[] dataValue;
                        // Replace the Illegal Opcodes with data statement
                        if (dataStatements.TryGetValue(i.ToString("X4"), out dataValue))
                        {
                            replacedWithDataCollection.Add(i.ToString("X4"), dataValue);
                        }
                    }

                    DialogResult result = DialogResult.Yes;
                    if (firstIllegalOpcodeFound)
                    {
                        result = MessageBox.Show("Illegal Opcodes found within the selection from : " + firstOccurance.ToString("X4") + " to " + lastOccurrance.ToString("X4") + "\n"
                        + "Replace Illegal Opcodes with data statements ?", " ", MessageBoxButtons.YesNo);
                    }

                    bool convertToBytes = false;
                    if (result == DialogResult.Yes)
                    {
                        convertToBytes = true;
                    }
                    AddLabels(ms.GetSelectedMemStartLocation, ms.GetSelectedMemEndLocation, convertToBytes, replacedWithDataCollection, firstOccurance, lastOccurrance);
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
            passOne.Clear();
            passTwo.Clear();
            passThree.Clear();
            found.Clear();
            labelLoc.Clear();
            branchLoc.Clear();
        }

        private void leftWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(code);
        }

        private void rightWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(passThree);
        }

        private void Save(List<string> collection)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save File",
                InitialDirectory = @"*.*",
                Filter = "All files (*.*)|*.*|All files (*.a)|*.a",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog.FileName, collection);
            }
        }

        private void generateLabelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            generateLabels();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearCollections();
            textBox1.Clear();
            textBox2.Clear();
            byteviewer.SetBytes(new byte[] { });
            //lineNumbers = 0;
        }
    }
}