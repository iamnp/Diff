using System.Drawing;
using System.Windows.Forms;
using Diff.Editor;
using Diff.Expressions;

namespace Diff.Plotter
{
    internal partial class Plotter : UserControl
    {
        private const int LineHeight = 50;

        private readonly Brush _backBrush = Brushes.White;

        public GlobalScope GlobalScope;

        public Plotter()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            VerticalScroll.SmallChange = LineHeight;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(_backBrush, ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (GlobalScope?.AssignmentStatements == null)
            {
                return;
            }
            for (var i = 0; i < GlobalScope.AssignmentStatements.Count; ++i)
            {
                e.Graphics.DrawLine(Pens.Black, 0, (float) (i + 1) * ExpressionEditor.LineHeight, Width,
                    (float) (i + 1) * ExpressionEditor.LineHeight);
                if (GlobalScope.AssignmentStatements[i].Assignee != null)
                {
                    for (var j = 0; j < GlobalScope.Iterations; ++j)
                    {
                        if (GlobalScope.AssignmentStatements[i].Assignee.Parent != null)
                        {
                            var child = GlobalScope.AssignmentStatements[i].Assignee.Parent.NthItem(j);
                            if (child.IsDouble)
                            {
                                var yPoint = child.AsDouble;
                                yPoint *= -ExpressionEditor.LineHeight / 2.0;
                                yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2;

                                e.Graphics.DrawRectangle(Pens.Black, j,
                                    (int) yPoint, 1, 1);
                            }
                        }
                        else if (GlobalScope.AssignmentStatements[i].Assignee.IsDouble)
                        {
                            var yPoint = GlobalScope.AssignmentStatements[i].Assignee.AsDouble;
                            yPoint *= -ExpressionEditor.LineHeight / 2.0;
                            yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2;

                            e.Graphics.DrawRectangle(Pens.Black, j,
                                (int) yPoint, 1, 1);
                        }
                    }
                }
            }
        }
    }
}