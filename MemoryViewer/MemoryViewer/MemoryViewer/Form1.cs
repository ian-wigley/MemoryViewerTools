using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace MemoryViewer
{
    public partial class Form1 : Form
    {
        private ByteViewer byteviewer;

        public Form1()
        {
            InitializeComponent();
            InitializeForm();
            byteviewer = new ByteViewer();
            byteviewer.Location = new Point(8, 66);
            byteviewer.Size = new Size(700, 338);
            byteviewer.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            byteviewer.SetBytes(new byte[] { });
            this.Controls.Add(byteviewer);
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void InitializeForm()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(680, 440);
            this.MinimumSize = new System.Drawing.Size(660, 400);
            this.Size = new System.Drawing.Size(680, 440);
            this.Name = "Byte Viewer Form";
            this.Text = "Byte Viewer Form";
            
            System.Windows.Forms.GroupBox group = new System.Windows.Forms.GroupBox();
            group.Location = new Point(418, 23);
            group.Size = new Size(220, 36);
            group.Text = "Display Mode";
            this.Controls.Add(group);

            System.Windows.Forms.RadioButton rbutton1 = new System.Windows.Forms.RadioButton();
            rbutton1.Location = new Point(6, 15);
            rbutton1.Size = new Size(46, 16);
            rbutton1.Text = "Auto";
            rbutton1.Checked = true;
            rbutton1.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton1);

            System.Windows.Forms.RadioButton rbutton2 = new System.Windows.Forms.RadioButton();
            rbutton2.Location = new Point(54, 15);
            rbutton2.Size = new Size(50, 16);
            rbutton2.Text = "ANSI";
            rbutton2.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton2);

            System.Windows.Forms.RadioButton rbutton3 = new System.Windows.Forms.RadioButton();
            rbutton3.Location = new Point(106, 15);
            rbutton3.Size = new Size(46, 16);
            rbutton3.Text = "Hex";
            rbutton3.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton3);

            System.Windows.Forms.RadioButton rbutton4 = new System.Windows.Forms.RadioButton();
            rbutton4.Location = new Point(152, 15);
            rbutton4.Size = new Size(64, 16);
            rbutton4.Text = "Unicode";
            rbutton4.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton4);
            this.ResumeLayout(false);
        }

        // Changes the display mode of the byte viewer according to the  
        // Text property of the RadioButton sender control. 
        private void changeByteMode(object sender, EventArgs e)
        {
            System.Windows.Forms.RadioButton rbutton =
                (System.Windows.Forms.RadioButton)sender;

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

        // Show a file selection dialog and cues the byte viewer to  
        // load the data in a selected file. 
        private void loadBytesFromFile(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            byteviewer.SetFile(ofd.FileName);
        }

        // Clear the bytes in the byte viewer. 
        private void clearBytes(object sender, EventArgs e)
        {
            byteviewer.SetBytes(new byte[] { });
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            byteviewer.SetFile(ofd.FileName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
