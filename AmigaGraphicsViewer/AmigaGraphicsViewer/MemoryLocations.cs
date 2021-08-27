using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace MemoryViewer
{
    public partial class MemoryLocations : Form
    {
        private IList<object> memValues = new List<object>();

        private string GetSelectedMemStartLocation { get { return comboBox1.Text + comboBox2.Text + comboBox3.Text + comboBox4.Text + comboBox5.Text; } }
        private string GetSelectedMemEndLocation { get { return comboBox6.Text + comboBox7.Text + comboBox8.Text + comboBox9.Text + comboBox10.Text; } }

        public int GetMemLowLoc { get { return lowValue; } }
        public int GetMemHighLoc { get { return highValue; } }

        private int lowValue = 0;
        private int highValue = 0;
        private int numberOfBytes = 0;

        public MemoryLocations(int length)
        {
            InitializeComponent();

            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            for (int i = 0; i < 16; i++)
            {
                memValues.Add(new { Text = i.ToString("X1"), Value = i });
            }

            InitialiseComboBoxes(comboBox1);
            InitialiseComboBoxes(comboBox2);
            InitialiseComboBoxes(comboBox3);
            InitialiseComboBoxes(comboBox4);
            InitialiseComboBoxes(comboBox5);

            InitialiseComboBoxes(comboBox6);
            InitialiseComboBoxes(comboBox7);
            InitialiseComboBoxes(comboBox8);
            InitialiseComboBoxes(comboBox9);
            InitialiseComboBoxes(comboBox10);

            comboBox1.Text = "0";
            comboBox2.Text = "0";
            comboBox3.Text = "0";
            comboBox4.Text = "0";
            comboBox5.Text = "0";

            numberOfBytes = length;
        }

        private void InitialiseComboBoxes(ComboBox comboBox)
        {
            comboBox.BindingContext = new BindingContext();
            comboBox.DisplayMember = "Text";
            comboBox.ValueMember = "Value";
            comboBox.DataSource = memValues;
        }


        private void button1_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(GetSelectedMemStartLocation, NumberStyles.HexNumber, null, out lowValue)
                && int.TryParse(GetSelectedMemEndLocation, NumberStyles.HexNumber, null, out highValue)
                && highValue > lowValue
                && highValue <= numberOfBytes)
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

    }
}
