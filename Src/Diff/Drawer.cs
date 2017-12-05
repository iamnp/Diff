using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Diff.Editor;
using Diff.Expressions;

namespace Diff
{
    internal class Drawer
    {
        private readonly Pen _blackPen = new Pen
        {
            Thickness = 1,
            Brush = Brushes.Black
        };

        private readonly GlobalScope _gs;
        private readonly MainGraphicOutput _mainGraphics;

        private readonly List<Point> _points = new List<Point>();
        private readonly Brush _whiteBrush = new SolidColorBrush(Colors.White);
        private bool _finished = true;
        private Point _firstPoint;
        public Rect HostRect;
        public double MouseX = -1;

        public Drawer(GlobalScope gs, MainGraphicOutput mainGraphics)
        {
            _gs = gs;
            _mainGraphics = mainGraphics;

            _blackPen.Freeze();
            _whiteBrush.Freeze();

            _mainGraphics.DrawingFunc = DrawScene;
        }

        private void DrawPath(Point p)
        {
            if (_finished)
            {
                _firstPoint = p;
                _finished = false;
            }
            else
            {
                _points.Add(p);
            }
        }

        private void StopPath(DrawingContext dc, Rect visible)
        {
            if (!_finished)
            {
                var geometry = new StreamGeometry();
                using (var ctx = geometry.Open())
                {
                    ctx.BeginFigure(_firstPoint, true, true);
                    if (_points.Count > 0)
                    {
                        const int safeShift = 10;
                        _points.Add(new Point(visible.Right + safeShift, _points[_points.Count - 1].Y));
                        _points.Add(new Point(visible.Right + safeShift, visible.Bottom + safeShift));
                        _points.Add(new Point(visible.Left - safeShift, visible.Bottom + safeShift));
                        _points.Add(new Point(visible.Left - safeShift, _points[0].Y));
                        ctx.PolyLineTo(_points, true, false);
                    }
                }
                geometry.Freeze();
                var visibleRect = new RectangleGeometry(visible);
                visibleRect.Freeze();
                dc.PushClip(visibleRect);
                dc.DrawGeometry(Brushes.LightGray, _blackPen, geometry);
                dc.Pop();
            }
            _points.Clear();
            _finished = true;
        }

        public void DrawScene(DrawingContext dc)
        {
            if (_gs?.AssignmentStatements == null)
            {
                return;
            }

            dc.DrawRectangle(_whiteBrush, null, HostRect);

            for (var i = 0; i < _gs.AssignmentStatements.Count; ++i)
            {
                double? value = null;
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
                                if ((int) MouseX == j)
                                {
                                    value = yPoint;
                                }
                                yPoint *= -ExpressionEditor.LineHeight / 2.0;
                                yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2;
                                DrawPath(new Point(j, yPoint));
                            }
                        }
                        else if (_gs.AssignmentStatements[i].Assignee.IsDouble)
                        {
                            var yPoint = _gs.AssignmentStatements[i].Assignee.AsDouble;
                            if ((int) MouseX == j)
                            {
                                value = yPoint;
                            }
                            yPoint *= -ExpressionEditor.LineHeight / 2.0;
                            yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2;
                            DrawPath(new Point(j, yPoint));
                        }
                    }
                }
                StopPath(dc, new Rect(0, i * ExpressionEditor.LineHeight, _mainGraphics.ActualWidth,
                    ExpressionEditor.LineHeight));
                if (value != null)
                {
                    var ft = new FormattedText(value.Value.ToString("F2"), CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Black);
                    dc.DrawText(ft, new Point(MouseX + 4, (i + 1) * ExpressionEditor.LineHeight - ft.Height - 3));
                }
                dc.DrawLine(_blackPen, new Point(0, (float) (i + 1) * ExpressionEditor.LineHeight), new Point(
                    _mainGraphics.ActualWidth,
                    (float) (i + 1) * ExpressionEditor.LineHeight));
            }

            if (MouseX >= 0)
            {
                dc.DrawLine(_blackPen, new Point(MouseX, 0), new Point(MouseX, _mainGraphics.ActualHeight));
            }
        }
    }
}