using System;
using System.Drawing;
using System.Windows.Forms;
using Diff.Reductions.CodeEditing;

namespace Diff.Reductions
{
    public partial class ReductionForm : Form
    {
        private readonly ReductionCompiler _reductionCompiler = new ReductionCompiler();

        public ReductionForm()
        {
            InitializeComponent();
            codeEditor1.TextChanged += CodeEditor1OnTextChanged;

            listBox1.Items.Add(new Reduction(null, "mean", @"double sum = 0.0;
for (int i = 0; i < selection.Length; ++i) {
    sum += selection[i];
}
return sum/selection.Length;"));
            listBox1.Items.Add(new Reduction(null, "min", @"double min = selection[0];
for (int i = 1; i < selection.Length; ++i) {
    if (selection[i] < min) min = selection[i];
}
return min;"));
            listBox1.Items.Add(new Reduction(null, "max", @"double max = selection[0];
for (int i = 1; i < selection.Length; ++i) {
    if (selection[i] > max) max = selection[i];
}
return max;"));

            listBox1.SelectedIndex = 0;
        }

        private void CodeEditor1OnTextChanged(object sender, EventArgs eventArgs)
        {
            var oldReduction = (Reduction) listBox1.Items[listBox1.SelectedIndex];
            var newReduction = _reductionCompiler.Compile(oldReduction.Name, codeEditor1.Text);
            if (_reductionCompiler.LastErrorText != null)
            {
                codeEditor1.AddMarker(new LineMarker
                {
                    Color = Color.Red,
                    Line = _reductionCompiler.LastErrorLine,
                    Text = _reductionCompiler.LastErrorText
                });
            }
            else
            {
                codeEditor1.RemoveAllMarkers();
                codeEditor1.TextChanged -= CodeEditor1OnTextChanged;
                listBox1.Items[listBox1.SelectedIndex] = newReduction;
                codeEditor1.TextChanged += CodeEditor1OnTextChanged;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var r = (Reduction) listBox1.Items[listBox1.SelectedIndex];
            codeEditor1.Text = r.Code;
        }
    }
}