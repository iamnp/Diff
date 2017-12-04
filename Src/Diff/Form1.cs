using System;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Diff.Editor;
using Diff.Expressions;
using FlowDirection = System.Windows.FlowDirection;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

// === FEATURES ===
// TODO добавить ползунок для изменения начального значения

// === FIXES ===
// TODO исправить раскрашивание области под графиком
// TODO разобраться с кол-во шагов (сейчас 1000)
// TODO Функцию отрисовки в Drawer, Drawer не extension methodы class, Drawer not-static
// TODO разобраться с HostRect и растягиванием
// TODO посмотреть почему убывает последний график на тестах

namespace Diff
{
    public partial class MainForm : Form
    {
        private static readonly Rect HostRect = new Rect(0, 0, 1000, 700);
        private readonly GlobalScope _gs = new GlobalScope();
        private readonly MainGraphicOutput _mainGraphics;
        private double _mouseX = -1;

        public MainForm()
        {
            InitializeComponent();
            _mainGraphics = new MainGraphicOutput {DrawingFunc = DrawScene};
            _mainGraphics.MouseMove += MainGraphicsOnMouseMove;
            _mainGraphics.MouseLeave += MainGraphicsOnMouseLeave;
            elementHost1.Child = _mainGraphics;
            //VerticalScroll.SmallChange = LineHeight;
        }

        private void MainGraphicsOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            _mouseX = -1;
            _mainGraphics.InvalidateVisual();
        }

        private void MainGraphicsOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            _mouseX = mouseEventArgs.GetPosition(_mainGraphics).X;
            _mainGraphics.InvalidateVisual();
        }

        private void DrawScene(DrawingContext dc)
        {
            if (_gs?.AssignmentStatements == null)
            {
                return;
            }

            dc.DrawRectangle(Drawer.WhiteBrush, null, HostRect);

            for (var i = 0; i < _gs.AssignmentStatements.Count; ++i)
            {
                double? value = null;
                dc.DrawLine(Drawer.BlackPen, new Point(0, (float) (i + 1) * ExpressionEditor.LineHeight), new Point(
                    _mainGraphics.ActualWidth,
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
                                if ((int) _mouseX == j)
                                {
                                    value = yPoint;
                                }
                                yPoint *= -ExpressionEditor.LineHeight / 2.0;
                                yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2;
                                dc.DrawPath(new Point(j, yPoint));
                            }
                        }
                        else if (_gs.AssignmentStatements[i].Assignee.IsDouble)
                        {
                            var yPoint = _gs.AssignmentStatements[i].Assignee.AsDouble;
                            if ((int) _mouseX == j)
                            {
                                value = yPoint;
                            }
                            yPoint *= -ExpressionEditor.LineHeight / 2.0;
                            yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2;
                            dc.DrawPath(new Point(j, yPoint));
                        }
                    }
                }
                dc.StopPath(new Rect(0, i * ExpressionEditor.LineHeight, _mainGraphics.ActualWidth,
                    ExpressionEditor.LineHeight));
                if (value != null)
                {
                    var ft = new FormattedText(value.Value.ToString("F2"), CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Black);
                    dc.DrawText(ft, new Point(_mouseX + 4, (i + 1) * ExpressionEditor.LineHeight - ft.Height - 3));
                }
            }

            if (_mouseX >= 0)
            {
                dc.DrawLine(Drawer.BlackPen, new Point(_mouseX, 0), new Point(_mouseX, _mainGraphics.ActualHeight));
            }
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