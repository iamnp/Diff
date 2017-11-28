using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Diff.Editor;
using Diff.Expressions;

// TODO не давать рисовать график за границами своей линии
// TODO разобраться с HostRect
// TODO Функцию отрисовки в Drawer
// TODO Drawer не extension methodы class
// TODO разобраться с кол-во шагов (сейчас 1000)

namespace Diff
{
    public partial class MainForm : Form
    {
        private static readonly Rect HostRect = new Rect(0, 0, 1000, 700);
        private readonly GlobalScope _gs = new GlobalScope();
        private readonly MainGraphicOutput _mainGraphics;

        public MainForm()
        {
            InitializeComponent();
            _mainGraphics = new MainGraphicOutput {DrawingFunc = DrawScene};
            elementHost1.Child = _mainGraphics;
            //VerticalScroll.SmallChange = LineHeight;
        }

        private void DrawScene(DrawingContext dc)
        {
            if (_gs?.AssignmentStatements == null)
            {
                return;
            }

            dc.DrawRectangle(Drawer.WhiteBrush, null, HostRect);

            dc.StartSession();

            for (var i = 0; i < _gs.AssignmentStatements.Count; ++i)
            {
                dc.DrawLine(Drawer.BlackPen, new Point(0, (float) (i + 1) * ExpressionEditor.LineHeight), new Point(
                    Width,
                    (float) (i + 1) * ExpressionEditor.LineHeight));
                if (_gs.AssignmentStatements[i].Assignee != null)
                {
                    for (var j = 0; j < GlobalScope.Iterations; ++j)
                    {
                        if (_gs.AssignmentStatements[i].Assignee.Parent != null)
                        {
                            var child = _gs.AssignmentStatements[i].Assignee.Parent.NthItem(j);
                            if (child.IsDouble)
                            {
                                var yPoint = child.AsDouble;
                                yPoint *= -ExpressionEditor.LineHeight / 2.0;
                                yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2;
                                dc.DrawPath(new Point(j, yPoint));
                            }
                        }
                        else if (_gs.AssignmentStatements[i].Assignee.IsDouble)
                        {
                            var yPoint = _gs.AssignmentStatements[i].Assignee.AsDouble;
                            yPoint *= -ExpressionEditor.LineHeight / 2.0;
                            yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2;
                            dc.DrawPath(new Point(j, yPoint));
                        }
                    }
                }
                dc.StopPath();
            }

            dc.FlushSession();
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
            _mainGraphics.InvalidateVisual();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text = "a = 1";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text =
                "a[n] = a[n-1]-0.05*b[n-1]\r\nb[n] = b[n-1]+0.05*a[n]\r\nc[n] = if(a[n] > 0, 1, -1)\r\nd[n] = d[n-1] + 0.03*c[n]";
        }
    }
}