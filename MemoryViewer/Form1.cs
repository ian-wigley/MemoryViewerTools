using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MemoryViewer
{
    public partial class Form1 : Form
    {
        //private ByteViewer byteviewer;
        private int width = 320;
        private int height = 256;
        private GroupBox outerGroup = new GroupBox();
        private byte[] globalBytes = { };

        public Form1()
        {
            InitializeComponent();
            InitializeForm();

            byteviewer = new ByteViewer();
            byteviewer.Location = new Point(60, 66);
            byteviewer.Size = new Size(740, 400);
            byteviewer.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            byteviewer.SetBytes(new byte[] { });
            //tabPage1.Controls.Add(byteviewer);
            outerGroup.Controls.Add(byteviewer);

            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void InitializeForm()
        {
            SuspendLayout();
            //ClientSize = new Size(680, 880); //440
            //MinimumSize = new Size(660, 800); //400
            //Size = new Size(680, 440);
            Name = "Byte Viewer Form";
            Text = "Byte Viewer Form";


            outerGroup.Location = new Point(8, 6);
            outerGroup.Name = "outerGroup";
            outerGroup.Size = new Size(760, 467);
            outerGroup.TabIndex = 5;
            outerGroup.TabStop = false;
            outerGroup.Text = "Memory";
            //group.Controls.Add(this.groupBox2);
            tabPage1.Controls.Add(outerGroup);

            // Create a group box around the different byte display modes
            GroupBox group = new GroupBox();
            group.Location = new Point(270, 23);
            group.Size = new Size(260, 36); //220
            group.Text = "Display Mode";
            //Controls.Add(group);
            //tabPage1.Controls.Add(group);
            outerGroup.Controls.Add(group);

            RadioButton rbutton1 = new RadioButton();
            rbutton1.Location = new Point(6, 15);
            rbutton1.Size = new Size(50, 16); //46
            rbutton1.Text = "Auto";
            rbutton1.Checked = true;
            rbutton1.Click += new EventHandler(changeByteMode);
            group.Controls.Add(rbutton1);

            RadioButton rbutton2 = new RadioButton();
            rbutton2.Location = new Point(58, 15); //54
            rbutton2.Size = new Size(50, 16);
            rbutton2.Text = "ANSI";
            rbutton2.Click += new EventHandler(changeByteMode);
            group.Controls.Add(rbutton2);

            RadioButton rbutton3 = new RadioButton();
            rbutton3.Location = new Point(110, 15);//106
            rbutton3.Size = new Size(46, 16);
            rbutton3.Text = "Hex";
            rbutton3.Click += new EventHandler(changeByteMode);
            group.Controls.Add(rbutton3);

            RadioButton rbutton4 = new RadioButton();
            rbutton4.Location = new Point(156, 15); //152
            rbutton4.Size = new Size(68, 16); //64
            rbutton4.Text = "Unicode";
            rbutton4.Click += new EventHandler(changeByteMode);
            group.Controls.Add(rbutton4);

            ResumeLayout(false);
        }

        // Changes the display mode of the byte viewer according to the  
        // Text property of the RadioButton sender control. 
        private void changeByteMode(object sender, EventArgs e)
        {
            RadioButton rbutton = (RadioButton)sender;

            DisplayMode mode;
            switch (rbutton.Text)
            {
                case "ANSI":
                    mode = DisplayMode.Ansi;
                    break;
                case "Hex":
                    mode = DisplayMode.Hexdump;
                    break;
                case "Unicode":
                    mode = DisplayMode.Unicode;
                    break;
                default:
                    mode = DisplayMode.Auto;
                    break;
            }

            // Sets the display mode.
            byteviewer.SetDisplayMode(mode);
        }

        //// Show a file selection dialog and cues the byte viewer to  
        //// load the data in a selected file. 
        //private void loadBytesFromFile(object sender, EventArgs e)
        //{
        //    OpenFileDialog ofd = new OpenFileDialog();
        //    if (ofd.ShowDialog() != DialogResult.OK)
        //    {
        //        return;
        //    }
        //    byteviewer.SetFile(ofd.FileName);
        //}

        //// Clear the bytes in the byte viewer. 
        //private void clearBytes(object sender, EventArgs e)
        //{
        //    byteviewer.SetBytes(new byte[] { });
        //}

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            byteviewer.SetFile(ofd.FileName);
            byte[] bytes = byteviewer.GetBytes();
            globalBytes = byteviewer.GetBytes();

            // Ensure there are enough bytes otherwise the Bitmap creation will fail
            if (bytes.Length < (width * height))
            {
                byte[] bitmapBytes = new byte[0x14000];
                // Copy the bytes from disk into an array of bytes big enough to display
                Array.Copy(bytes, bitmapBytes, bytes.Length);
                bytes = new byte[bitmapBytes.Length];
                Array.Copy(bitmapBytes, bytes, bitmapBytes.Length);
            }

            canvas.BackgroundImage = new Bitmap(width, height, width, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0));

            // Process filename to display within the form
            Name = ofd.FileName;
            string temp = ofd.FileName;
            int start = temp.LastIndexOf("\\") + 1;
            //Text = temp.Substring(temp.LastIndexOf("\\") + 1, temp.Length - temp.LastIndexOf("\\") + 1) + " - Byte Viewer";
            Text = temp.Substring(start, temp.Length - start) + " - Byte Viewer";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "IFF file (*.iff)|*.iff|CSV file (*.csv)|*.csv|Binary file (*.bin)|*.bin|Text file (*.txt)|*.txt|All files (*.*)|*.*";// ".csv";
            sfd.AddExtension = true;

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }


            byte[] bytes = byteviewer.GetBytes();

            int len = bytes.Length;

            if (sfd.FileName.ToLower().Contains(".bin"))
            {
                DialogResult result = MessageBox.Show("Do you want to save a chunk of memory ?", "Oy !", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    MemoryLocations ms = new MemoryLocations(len);
                    if (ms.ShowDialog() == DialogResult.OK)
                    {
                        //int one = ms.GetMemLowLoc;
                        //int two = ms.GetMemHighLoc;
                        CopyDataAndWrite(ms.GetMemLowLoc, ms.GetMemHighLoc, bytes, sfd.FileName);
                    }
                }
                //// Create a copy of the original byte array as specified by the user
                //byte[] userSelecion = new byte[0x10000];
                //int count = 0;
                //for(int i = 0x31000; i < (0x31000 + 0x10000); i++)
                //{
                //    userSelecion[count++] = bytes[i];
                //}
                //File.WriteAllBytes(sfd.FileName, userSelecion);
            }


            if (sfd.FileName.ToLower().Contains(".csv"))
            {
                //byte[] copy = new byte[0x4c];// bytes.Length - 0x0853];
                //int count = 0;
                //for (int i = 0x0854; i < 0x8a0; i++) // bytes.Length; i++)
                //{
                //    copy[count++] = bytes[i];
                //}
                List<string> datawords = new List<string>();
                for (int i = 0; i < bytes.Length - 4; i += 4) // bytes.Length; i++)
                {
                    //string hex = bytes[i].ToString("X2");// + bytes[i + 1].ToString("X2");// "03c";//x3c";
                    var s = SeperateString(string.Join("", bytes[i].ToString("X2").Select(x => Convert.ToString(Convert.ToInt32(x + "", 16), 2).PadLeft(4, '0'))));
                    var t = SeperateString(string.Join("", bytes[i + 1].ToString("X2").Select(x => Convert.ToString(Convert.ToInt32(x + "", 16), 2).PadLeft(4, '0'))));
                    var u = SeperateString(string.Join("", bytes[i + 2].ToString("X2").Select(x => Convert.ToString(Convert.ToInt32(x + "", 16), 2).PadLeft(4, '0'))));
                    var v = SeperateString(string.Join("", bytes[i + 3].ToString("X2").Select(x => Convert.ToString(Convert.ToInt32(x + "", 16), 2).PadLeft(4, '0'))));

                    datawords.Add(s + t + " " + u + v);

                    //datawords.Add("dc.w $" + bytes[i].ToString("X2") + bytes[i + 1].ToString("X2")
                    //    + ",$" + bytes[i + 2].ToString("X2") + bytes[i + 3].ToString("X2")
                    //    + " %" + s
                    //    + " %" + t
                    //    + " %" + u
                    //    + " %" + v
                    //    );
                }

                //            string binary = "%" + string.Join(", %", bytes.Select(x => Convert.ToString(x, 2/*NumberFormatInfo.InvariantInfo*/).PadLeft(8, '0') + "\n"));

                //            string hex = "03c";//x3c";
                //            var s = string.Join("", hex.Select(x => Convert.ToString(Convert.ToInt32(x + "", 16), 2).PadLeft(4, '0')));

                File.WriteAllLines(sfd.FileName, datawords);
            }
            if (sfd.FileName.ToLower().Contains(".txt"))
            {
                string[] stringArray = new string[bytes.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringArray[i] = bytes[i].ToString();
                }
                //File.WriteAllLines(sfd.FileName, stringArray);
                File.WriteAllBytes(sfd.FileName, bytes);
            }

            if (sfd.FileName.ToLower().Contains(".iff"))
            {

                BuildIFFandWrite(sfd.FileName, byteSelection);// bytes);
            }
        }
        private string SeperateString(string input)
        {
            string test = "";
            foreach (char c in input)
            {
                test += c + ",";
            }
            return test;
        }

        int imagePointer = 0;

        //private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        //{
        //    imagePointer += 320;
        //    byte[] bytes = byteviewer.GetBytes();
        //    Bitmap bm = new Bitmap(320, 256, 320, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, Marshal.UnsafeAddrOfPinnedArrayElement(bytes, imagePointer));
        //    canvas.BackgroundImage = bm;
        //}

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (byteviewer.GetBytes().Length > (width * height))
            {
                //string s = vScrollBar1.Value.ToString();
                byte[] bytes = byteviewer.GetBytes();

                // vScrollBar1.SmallChange = 1;
                // vScrollBar1.LargeChange = 1;// * 6;
                vScrollBar1.Maximum = bytes.Length - (width * height);// * 3200;

                if (bytes.Length > 0)
                {
                    if (e.NewValue > e.OldValue)
                    {
                        imagePointer += width;
                    }
                    else if (e.NewValue < e.OldValue)
                    {
                        imagePointer -= width;
                    }

                    Bitmap bm = new Bitmap(width, height, width, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, Marshal.UnsafeAddrOfPinnedArrayElement(bytes, imagePointer));
                    canvas.BackgroundImage = bm;
                    label1.Text = imagePointer.ToString("X6");
                }
            }
        }

        // Method to Copy the user selected data into a new array and write to disk
        private void CopyDataAndWrite(int start, int end, byte[] bytes, string filename)
        {
            byte[] userSelecion = new byte[end - start];
            int count = 0;
            for (int i = start; i < end; i++)
            {
                userSelecion[count++] = bytes[i];
            }
            File.WriteAllBytes(filename, userSelecion);
        }

        public struct BitmapHeader
        {
            //public byte bmhdsize;// = 20;
            public ushort width, height;
            public short x, y;
            public byte nPlanes;
            public byte masking;
            public byte compression;
            public byte pad1;
            public ushort transparentColour;
            public byte xAspect, yAspect;
            public short pageWidth, pageHeight;
        }

        private void BuildIFFandWrite(string filename, byte[] bytes)
        {
            IFF iff = new IFF(bytes, 1);
            iff.BuildChunks();
            iff.UpdateChunks(1);
            iff.GetChunkData();
            iff.WriteIFF(filename);

            //int idSizes = 4;
            //byte[] iffData = { };
            //byte[] formText = { 0x46, 0x4f, 0x52, 0x4d };
            //byte[] formSize = SwapBytes(BitConverter.GetBytes(bytes.Length));
            //// Size of the overall file in bytes e.g. 0x0, 0x0, 0x3, 0x6a, 
            //// fileSize
            //byte[] ilbmText = { 0x49, 0x4c, 0x42, 0x4d };
            //byte[] bmhdText = { 0x42, 0x4d, 0x48, 0x44 };
            //byte[] bmhdSize = { 0x0, 0x0, 0x0, 0x14 };
            ////byte[] bmhdSize = BitConverter.GetBytes(20);
            //byte[] cmapText = { 0x43, 0x4d, 0x41, 0x50 };
            //byte[] cmapSize = SwapBytes(BitConverter.GetBytes(20));
            //byte[] cmapData = { 0xf0, 0xf0, 0xf0, 0x0, 0x0, 0x0 };

            //byte[] bodyText = { 0x42, 0x4f, 0x44, 0x59 };
            //byte[] bodySize = SwapBytes(BitConverter.GetBytes(20));

            //BitmapHeader bmhd;
            //bmhd.width = 320;
            //bmhd.height = 256;
            //bmhd.x = 0;
            //bmhd.y = 0;
            //bmhd.nPlanes = 1;
            //bmhd.masking = 0;
            //bmhd.compression = 0;
            //bmhd.pad1 = 0;
            //bmhd.transparentColour = 0;
            //bmhd.xAspect = 0;
            //bmhd.yAspect = 0;
            //bmhd.pageWidth = 0;
            //bmhd.pageHeight = 0;

            //int count = 0;
            //iffData = new byte[idSizes + formSize.Length + idSizes + idSizes + 20 /*????bmhdContent.Count*/ + idSizes + cmapSize.Length + cmapData.Length + idSizes + bodySize.Length + bytes.Length];

            //for (int i = 0; i < formText.Length; i++)
            //{
            //    iffData[count++] = formText[i];
            //}
            //for (int i = 0; i < formSize.Length; i++)
            //{
            //    iffData[count++] = formSize[i];
            //}

            //for (int i = 0; i < ilbmText.Length; i++)
            //{
            //    iffData[count++] = ilbmText[i];
            //}
            //for (int i = 0; i < bmhdText.Length; i++)
            //{
            //    iffData[count++] = bmhdText[i];
            //}
            //for (int i = 0; i < bmhdSize.Length; i++)
            //{
            //    iffData[count++] = bmhdSize[i];
            //}

            //ConvertUShortToTwoBytes(bmhd.width, ref iffData, ref count);// = 300; // SwapBytes(BitConverter.GetBytes(300));
            //ConvertUShortToTwoBytes(bmhd.height, ref iffData, ref count);// = 256; // SwapBytes(BitConverter.GetBytes(256));
            //ConvertShortToTwoBytes(bmhd.x, ref iffData, ref count);
            //ConvertShortToTwoBytes(bmhd.y, ref iffData, ref count);
            //iffData[count++] = bmhd.nPlanes;
            //iffData[count++] = bmhd.masking;
            //iffData[count++] = bmhd.compression;
            //iffData[count++] = bmhd.pad1;
            //ConvertUShortToTwoBytes(bmhd.transparentColour, ref iffData, ref count);
            //iffData[count++] = bmhd.xAspect;
            //iffData[count++] = bmhd.yAspect;
            //ConvertShortToTwoBytes(bmhd.pageWidth, ref iffData, ref count);
            //ConvertShortToTwoBytes(bmhd.pageHeight, ref iffData, ref count);

            //// CMAP data ...
            //for (int i = 0; i < cmapText.Length; i++)
            //{
            //    iffData[count++] = cmapText[i];
            //}
            //for (int i = 0; i < cmapSize.Length; i++)
            //{
            //    iffData[count++] = cmapSize[i];
            //}
            //for (int i = 0; i < cmapData.Length; i++)
            //{
            //    iffData[count++] = cmapData[i];
            //}


            //for (int i = 0; i < bodyText.Length; i++)
            //{
            //    iffData[count++] = bodyText[i];
            //}
            //for (int i = 0; i < bodySize.Length; i++)
            //{
            //    iffData[count++] = bodySize[i];
            //}
            //for (int i = 250; i < bytes.Length - 200; i++)
            //{
            //    iffData[count++] = bytes[i];
            //}
            //File.WriteAllBytes(filename, iffData);
        }

        private void ConvertUShortToTwoBytes(ushort input, ref byte[] bytes, ref int index)
        {
            byte[] ushortBytes = BitConverter.GetBytes(input);
            bytes[index++] = ushortBytes[1];
            bytes[index++] = ushortBytes[0];
        }

        private void ConvertShortToTwoBytes(short input, ref byte[] bytes, ref int index)
        {
            byte[] shortBytes = BitConverter.GetBytes(input);
            bytes[index++] = shortBytes[1];
            bytes[index++] = shortBytes[0];
        }

        byte[] SwapBytes(byte[] input)
        {
            return new byte[] { input[3], input[2], input[1], input[0] };
        }

        Font fnt = new Font("Arial", 10);

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //g.DrawString("this is a diagonal line", fnt, Brushes.Blue, new Point(30, 30));
            ////g.DrawLine(Pens.Red, 0, 0, 300, 300);
            //g.DrawLine(Pens.Red, tabControl1.Left, tabControl1.Top, tabControl1.Right, tabControl1.Bottom);
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //Graphics g = e.Graphics;
            //g.DrawString("this is a diagonal line", fnt, System.Drawing.Brushes.Blue, new Point(30, 30));
            //g.DrawLine(Pens.Red, tabControl1.Left, tabControl1.Top, tabControl1.Right, tabControl1.Bottom);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IFFDetail iffDetail = new IFFDetail();
            if (iffDetail.ShowDialog() == DialogResult.OK)
            {
                int j = 0;
            }

                //int memLoc = int.Parse(label1.Text, NumberFormatInfo.InvariantInfo);
                string testText = "0x46f00";
            int result = 0;
            int.TryParse(testText.Substring(2), NumberStyles.AllowHexSpecifier, null, out result);
            int index = 0;
            byteSelection = new byte[40 * 256];
            if (result < globalBytes.Length)
            {
                for (int i = result; i < result + (40 * 256); i++)
                {
                    byteSelection[index++] = globalBytes[i];
                }
            }
         }

        private byte[] byteSelection = { };

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // To-do - add find functionality to search for stuff within the data set !
            int i = 0;
        }
    }
}
