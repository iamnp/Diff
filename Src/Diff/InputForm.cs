using System;
using System.Windows.Forms;

namespace Diff
{
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }

        public string InputText
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public string LabelText
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Return)
            {
                button1.PerformClick();
                e.Handled = true;
            }
        }
    }
}