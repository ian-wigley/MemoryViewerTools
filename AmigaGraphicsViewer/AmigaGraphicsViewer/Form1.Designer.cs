using System.Windows.Forms;
using System.ComponentModel.Design;

namespace MemoryViewer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private ByteViewer byteviewer = new ByteViewer();
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bmapHeight = new System.Windows.Forms.Label();
            this.bmapWidth = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.CopyDataLabel = new System.Windows.Forms.Label();
            this.Copy = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.VScrollBar = new System.Windows.Forms.VScrollBar();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1045, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadMenuClick);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(143, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitMenuClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(49, 24);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(120, 26);
            this.findToolStripMenuItem.Text = "Find";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.FindToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 33);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1045, 1006);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControl1_DrawItem);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(1037, 977);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Memory Viewer";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(1037, 977);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Bitmap Viewer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bmapHeight);
            this.groupBox1.Controls.Add(this.bmapWidth);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.CopyDataLabel);
            this.groupBox1.Controls.Add(this.Copy);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.VScrollBar);
            this.groupBox1.Controls.Add(this.canvas);
            this.groupBox1.Location = new System.Drawing.Point(11, 7);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1013, 575);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Memory";
            // 
            // bmapHeight
            // 
            this.bmapHeight.AutoSize = true;
            this.bmapHeight.Location = new System.Drawing.Point(25, 114);
            this.bmapHeight.Name = "bmapHeight";
            this.bmapHeight.Size = new System.Drawing.Size(53, 17);
            this.bmapHeight.TabIndex = 11;
            this.bmapHeight.Text = "Height:";
            // 
            // bmapWidth
            // 
            this.bmapWidth.AutoSize = true;
            this.bmapWidth.Location = new System.Drawing.Point(25, 92);
            this.bmapWidth.Name = "bmapWidth";
            this.bmapWidth.Size = new System.Drawing.Size(48, 17);
            this.bmapWidth.TabIndex = 10;
            this.bmapWidth.Text = "Width:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "Bitmap size:";
            // 
            // CopyDataLabel
            // 
            this.CopyDataLabel.AutoSize = true;
            this.CopyDataLabel.Location = new System.Drawing.Point(432, 458);
            this.CopyDataLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CopyDataLabel.Name = "CopyDataLabel";
            this.CopyDataLabel.Size = new System.Drawing.Size(162, 17);
            this.CopyDataLabel.TabIndex = 8;
            this.CopyDataLabel.Text = "Copy Screen Data to IFF";
            // 
            // Copy
            // 
            this.Copy.Location = new System.Drawing.Point(459, 478);
            this.Copy.Margin = new System.Windows.Forms.Padding(4);
            this.Copy.Name = "Copy";
            this.Copy.Size = new System.Drawing.Size(100, 28);
            this.Copy.TabIndex = 7;
            this.Copy.Text = "Copy";
            this.Copy.UseVisualStyleBackColor = true;
            this.Copy.Click += new System.EventHandler(this.Button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(228, 46);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "->";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 48);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Memory Location :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(157, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "0x00000";
            // 
            // VScrollBar
            // 
            this.VScrollBar.LargeChange = 1;
            this.VScrollBar.Location = new System.Drawing.Point(701, 111);
            this.VScrollBar.Name = "VScrollBar";
            this.VScrollBar.Size = new System.Drawing.Size(22, 214);
            this.VScrollBar.TabIndex = 3;
            this.VScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.VScrollBar_Scroll);
            // 
            // canvas
            // 
            this.canvas.Location = new System.Drawing.Point(253, 52);
            this.canvas.Margin = new System.Windows.Forms.Padding(4);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(427, 315);
            this.canvas.TabIndex = 1;
            this.canvas.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 100);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 1035);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Memory Viewer";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage tabPage2;
        private GroupBox groupBox1;
        private Label label1;
        private VScrollBar VScrollBar;
        private PictureBox canvas;
        private TabPage tabPage1;
        private GroupBox groupBox2;
        private Label label3;
        private Label label2;
        private Label CopyDataLabel;
        private Button Copy;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem findToolStripMenuItem;
        private Label bmapHeight;
        private Label bmapWidth;
        private Label label5;
    }
}

