﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Diff.Reductions.CodeEditing;
using Diff.Reductions.Compilation;

namespace Diff.Reductions
{
    public partial class ReductionForm : Form
    {
        private const string MeanReduction = @"double sum = 0.0;
for (int i = 0;
     i < selection.Length;
     ++i) {
    sum += selection[i];
}
return sum/selection.Length;";

        private const string MinReduction = @"double min = selection[0];
for (int i = 1;
     i < selection.Length;
     ++i) {
    if (selection[i] < min)
        min = selection[i];
}
return min;";

        private const string MaxReduction = @"double max = selection[0];
for (int i = 1;
     i < selection.Length;
     ++i) {
    if (selection[i] > max)
        max = selection[i];
}
return max;";

        private readonly ReductionCompiler _reductionCompiler = new ReductionCompiler(SynchronizationContext.Current);

        public ReductionForm()
        {
            InitializeComponent();

            _reductionCompiler.Compiled += ReductionCompilerOnCompiled;
            _reductionCompiler.CompilationError += ReductionCompilerOnCompilationError;

            codeEditor1.TextChanged += CodeEditor1OnTextChanged;

            listBox1.Items.Add(new Reduction(null, "mean", MeanReduction));
            listBox1.Items.Add(new Reduction(null, "min", MinReduction));
            listBox1.Items.Add(new Reduction(null, "max", MaxReduction));

            listBox1.SelectedIndex = 0;
        }

        public Reduction SelectedReduction
        {
            get
            {
                if (listBox1.SelectedIndex < 0)
                {
                    return null;
                }

                return (Reduction) listBox1.Items[listBox1.SelectedIndex];
            }
        }

        public event EventHandler ReductionChanged;

        public void MoveBelowCursor()
        {
            Location = new Point(Cursor.Position.X - Width / 2, Cursor.Position.Y + 2);
        }

        private void ReductionCompilerOnCompilationError(object sender, CompilationErrorEventArgs e)
        {
            codeEditor1.RemoveAllMarkers();
            codeEditor1.AddMarker(new LineMarker
            {
                Color = Color.Red,
                Line = e.Line,
                Text = e.ErrorText
            });
        }

        private void ReductionCompilerOnCompiled(object sender, CompiledEventArgs e)
        {
            codeEditor1.RemoveAllMarkers();

            for (var i = 0; i < listBox1.Items.Count; i++)
            {
                if (((Reduction) listBox1.Items[i]).Name == e.Reduction.Name)
                {
                    codeEditor1.TextChanged -= CodeEditor1OnTextChanged;
                    listBox1.Items[i] = e.Reduction;
                    codeEditor1.TextChanged += CodeEditor1OnTextChanged;
                    ReductionChanged?.Invoke(this, EventArgs.Empty);
                    break;
                }
            }
        }

        private void CodeEditor1OnTextChanged(object sender, EventArgs eventArgs)
        {
            if (codeEditor1.Text == "")
            {
                codeEditor1.Enabled = false;
                ReductionChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                codeEditor1.Enabled = true;
                var oldReduction = (Reduction) listBox1.Items[listBox1.SelectedIndex];
                _reductionCompiler.Compile(oldReduction.Name, codeEditor1.Text);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                var r = (Reduction) listBox1.Items[listBox1.SelectedIndex];
                codeEditor1.Text = r.Code;
            }
            else
            {
                codeEditor1.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                var newIndex = listBox1.SelectedIndex;
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                if (listBox1.Items.Count > newIndex)
                {
                    listBox1.SelectedIndex = newIndex;
                }
                else if (listBox1.Items.Count > 0)
                {
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new InputForm
            {
                Text = "Diff",
                LabelText = "Enter reduction name",
                StartPosition = FormStartPosition.CenterParent
            };
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                listBox1.Items.Add(new Reduction(null, form.InputText, MeanReduction));
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }

        private void ReductionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ReductionForm_Shown(object sender, EventArgs e)
        {
            Debug.WriteLine("JHErfE");
        }
    }
}