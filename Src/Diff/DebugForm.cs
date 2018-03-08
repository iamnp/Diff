using System;
using System.Drawing;
using System.Windows.Forms;
using Diff.Editor;

namespace Diff
{
    internal partial class DebugForm : Form
    {
        private readonly ExpressionEditor _ee;
        private readonly CueTextBox _searchBox;

        public DebugForm(ExpressionEditor ee, CueTextBox searchBox)
        {
            InitializeComponent();

            _ee = ee;
            _searchBox = searchBox;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _ee.Text = "a = 1.00";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _ee.Text =
                "a[n+1] = a[n]-0.050*b[n]\r\nb[n+1] = b[n]+0.050*a[n+1]\r\nc[n+1] = if(a[n] > 0, 1, -1)\r\nd[n+1] = d[n] + 0.030*c[n]";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _searchBox.Text = "a[n+1] > a[n]";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _ee.Text =
                "a = -0.9\r\nb = -0.8\r\nc = -0.7\r\nd = -0.6\r\ne = -0.5\r\nf = -0.4\r\ng = -0.3\r\nh = -0.2\r\ni = -0.1\r\nj = 0\r\nk = 0.1\r\nl = 0.2\r\nm = 0.3\r\nn = 0.4\r\no = 0.5\r\np = 0.6\r\nq = 0.7\r\nr = 0.8\r\ns = 0.9";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _ee.Text = "a[n+1] = a[n]-0.05\r\nb[n] = der(a[n])";
        }

        private void DebugForm_Shown(object sender, EventArgs e)
        {
            Location = new Point(0, 0);
        }
    }
}