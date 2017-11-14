using System;
using System.Windows.Forms;
using Diff.Expressions;

namespace Diff
{
    public partial class MainForm : Form
    {
        private readonly GlobalScope _gs = new GlobalScope();

        public MainForm()
        {
            InitializeComponent();
            plotter1.GlobalScope = _gs;
        }

        private void expressionEditor1_TextChanged(object sender, EventArgs e)
        {
            expressionEditor1.RemoveAllMarkers();
            var invalidStatements = _gs.UpdateAssignmentStatementsList(expressionEditor1.Text.Split('\n'));
            foreach (var m in invalidStatements)
            {
                expressionEditor1.AddMarker(m);
            }

            var marker = _gs.Evaluate();
            if (marker != null)
            {
                expressionEditor1.AddMarker(marker);
            }
            plotter1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text = "a = 1\r\nb = 0";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text = "a[n] = a[n-1]-0.05*b[n-1]\r\nb[n] = b[n-1]+0.05*a[n]";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text =
                "a[n] = a[n-1]-0.05*b[n-1]\r\nb[n] = b[n-1]+0.05*a[n]\r\nc[n] = if(a[n] > 0, 1, -1)\r\nd = 0";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text =
                "a[n] = a[n-1]-0.05*b[n-1]\r\nb[n] = b[n-1]+0.05*a[n]\r\nc[n] = if(a[n] > 0, 1, -1)\r\nd[n] = d[n-1] + 0.03*c[n]";
        }
    }
}