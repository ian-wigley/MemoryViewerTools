using System.Collections.Generic;
using System.Windows.Forms;

namespace MemoryViewer
{
    public partial class IFFDetail : Form
    {
        private IList<object> memValues = new List<object>();
        public IFFDetail()
        {
            InitializeComponent();
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;


            memValues.Add(1);
            memValues.Add(2);
            memValues.Add(3);
            memValues.Add(4);
            memValues.Add(5);

            numberBitplanes.BindingContext = new BindingContext();
            numberBitplanes.DisplayMember = "Text";
            numberBitplanes.ValueMember = "Value";
            numberBitplanes.DataSource = memValues;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            //if (int.TryParse(GetSelectedMemStartLocation, NumberStyles.HexNumber, null, out lowValue)
            //&& int.TryParse(GetSelectedMemEndLocation, NumberStyles.HexNumber, null, out highValue)
            //&& highValue > lowValue
            //&& highValue <= numberOfBytes)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
