using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Diff.Editor;
using Diff.Expressions;
using Diff.Manipulators;

namespace Diff
{
    internal class Drawer
    {
        public const int LeftOffset = 50;
        public const int TopOffset = 20;
        private static readonly Typeface ArialTypeface = new Typeface("Arial");

        private readonly Pen _blackPen = new Pen
        {
            Thickness = 1,
            Brush = Brushes.Black
        };

        private readonly Pen _bottomLinePen = new Pen
        {
            Thickness = 1,
            Brush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
        };

        private readonly Brush _graphFillBrush = new SolidColorBrush(Color.FromArgb(140, 160, 106, 58));

        private readonly GlobalScope _gs;
        private readonly Brush _hoveredSearchAreaBrush = new SolidColorBrush(Color.FromArgb(70, 165, 116, 80));
        private readonly Brush _initialValueManipulatorColor = new SolidColorBrush(Color.FromRgb(255, 218, 122));
        private readonly MainGraphicOutput _mainGraphics;
        private readonly Manipulator _manipulator;

        private readonly List<Point> _points = new List<Point>();

        private readonly Brush _rowBackgroundBrush =
            new LinearGradientBrush(Color.FromRgb(255, 218, 122), Color.FromRgb(239, 176, 83), 90);

        private readonly Brush _searchAreaBrush = new SolidColorBrush(Color.FromArgb(50, 200, 146, 98));

        private readonly Pen _searchIntervalBorderPen =
            new Pen(new SolidColorBrush(Color.FromArgb(190, 149, 106, 78)), 2);

        private readonly Brush _selectedSearchAreaBrush = new SolidColorBrush(Color.FromArgb(90, 165, 116, 80));

        private readonly Point _topLeftOffset = new Point(LeftOffset, TopOffset);
        private readonly Brush _whiteBrush = new SolidColorBrush(Colors.White);
        private bool _finished = true;
        private Point _firstPoint;
        private Point[] _initialPoints;
        private double?[] _initialValues;
        public Rect HostRect;
        public int VerticalScroll = 0;

        public Drawer(GlobalScope gs, MainGraphicOutput mainGraphics, Manipulator manipulator)
        {
            _gs = gs;
            _mainGraphics = mainGraphics;
            _manipulator = manipulator;

            _initialPoints = new Point[_gs.AssignmentStatements.Count];
            _initialValues = new double?[_gs.AssignmentStatements.Count];

            _blackPen.Freeze();
            _whiteBrush.Freeze();
            _searchAreaBrush.Freeze();
            _selectedSearchAreaBrush.Freeze();
            _hoveredSearchAreaBrush.Freeze();
            _rowBackgroundBrush.Freeze();
            _graphFillBrush.Freeze();
            _bottomLinePen.Freeze();
            _searchIntervalBorderPen.Freeze();

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
                dc.DrawGeometry(_graphFillBrush, null, geometry);
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
            var tt2 = new TranslateTransform(0, TopOffset);
            tt2.Freeze();
            dc.PushTransform(tt2);
            var tt1 = new TranslateTransform(LeftOffset, 0);
            tt1.Freeze();
            dc.PushTransform(tt1);

            if ((_initialPoints == null) || (_initialPoints.Length != _gs.AssignmentStatements.Count))
            {
                _initialPoints = new Point[_gs.AssignmentStatements.Count];
            }

            if ((_initialValues == null) || (_initialValues.Length != _gs.AssignmentStatements.Count))
            {
                _initialValues = new double?[_gs.AssignmentStatements.Count];
            }

            for (var i = 0; i < _gs.AssignmentStatements.Count; ++i)
            {
                var bottomLinePos = (i + 1) * ExpressionEditor.LineHeight - VerticalScroll;

                if (bottomLinePos < ExpressionEditor.LineHeight)
                {
                    dc.DrawLine(_bottomLinePen, new Point(0, bottomLinePos),
                        new Point(_mainGraphics.ActualWidth, bottomLinePos));
                    continue;
                }

                dc.DrawRectangle(_rowBackgroundBrush, null,
                    new Rect(new Point(0, i * ExpressionEditor.LineHeight - VerticalScroll),
                        new Point(_mainGraphics.ActualWidth, (i + 1) * ExpressionEditor.LineHeight - VerticalScroll)));

                double? value = null;
                var set = false;
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
                                if ((int) _manipulator.InitialValueManipulator.MouseX - LeftOffset == j)
                                {
                                    value = yPoint;
                                }

                                var v = yPoint;
                                yPoint *= -ExpressionEditor.LineHeight / 2.0;
                                yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2 -
                                          VerticalScroll;
                                var p = new Point(j, yPoint);
                                DrawPath(p);
                                if (!set)
                                {
                                    set = true;
                                    _initialValues[i] = v;
                                    _initialPoints[i] = p;
                                }
                            }
                        }
                        else if (_gs.AssignmentStatements[i].Assignee.IsDouble)
                        {
                            var yPoint = _gs.AssignmentStatements[i].Assignee.AsDouble;
                            if ((int) _manipulator.InitialValueManipulator.MouseX - LeftOffset == j)
                            {
                                value = yPoint;
                            }

                            var v = yPoint;
                            yPoint *= -ExpressionEditor.LineHeight / 2.0;
                            yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2 -
                                      VerticalScroll;
                            var p = new Point(j, yPoint);
                            DrawPath(p);
                            if (!set)
                            {
                                set = true;
                                _initialValues[i] = v;
                                _initialPoints[i] = p;
                            }
                        }
                    }
                }

                StopPath(dc, new Rect(0, i * ExpressionEditor.LineHeight - VerticalScroll, _mainGraphics.ActualWidth,
                    ExpressionEditor.LineHeight));
                if ((value != null) && (_manipulator.InitialValueManipulator.MouseX > LeftOffset))
                {
                    var ft = new FormattedText(value.Value.ToString("F2"), CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, ArialTypeface, 12, Brushes.Black);
                    dc.DrawText(ft,
                        new Point(_manipulator.InitialValueManipulator.MouseX + 4 - LeftOffset,
                            (i + 1) * ExpressionEditor.LineHeight - ft.Height - 3 - VerticalScroll));
                }

                dc.DrawLine(_bottomLinePen, new Point(0, bottomLinePos),
                    new Point(_mainGraphics.ActualWidth, bottomLinePos));
            }

            DrawSearchIntervals(dc);

            if (_manipulator.InitialValueManipulator.MouseX > LeftOffset)
            {
                dc.DrawLine(_blackPen, new Point(_manipulator.InitialValueManipulator.MouseX - LeftOffset, 0),
                    new Point(_manipulator.InitialValueManipulator.MouseX - LeftOffset, _mainGraphics.ActualHeight));
                var ft = new FormattedText((_manipulator.InitialValueManipulator.MouseX - LeftOffset).ToString(),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, ArialTypeface, 12, Brushes.Black);
                dc.DrawText(ft,
                    new Point(_manipulator.InitialValueManipulator.MouseX - LeftOffset + 4, 4));
            }

            dc.Pop();
            DrawInitialValueManipulator(dc, _initialPoints, _initialValues);

            dc.Pop();
            DrawUpperScale(dc);

            DrawSelectedIntervalLength(dc);
        }

        private void DrawSelectedIntervalLength(DrawingContext dc)
        {
            if (_gs.SelectedInterval != null)
            {
                var len = _gs.SelectedInterval.End - _gs.SelectedInterval.Start + 1;
                var ft = new FormattedText("Δn = " + len.ToString("D"), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, ArialTypeface, 12, Brushes.Black);
                dc.DrawText(ft, new Point(0, 0));
            }
        }

        private void DrawUpperScale(DrawingContext dc)
        {
            dc.DrawLine(_blackPen, _topLeftOffset, new Point(_mainGraphics.ActualWidth, TopOffset));
            for (var j = 0; j < GlobalScope.Iterations; ++j)
            {
                if (j % 20 == 0)
                {
                    if (j % 100 == 0)
                    {
                        var ft = new FormattedText(j.ToString("D"), CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight, ArialTypeface, 12, Brushes.Black);
                        dc.DrawText(ft, new Point(j + LeftOffset + 2, 0));
                        dc.DrawLine(_blackPen, new Point(j + LeftOffset, 5), new Point(j + LeftOffset, 20));
                    }
                    else
                    {
                        dc.DrawLine(_blackPen, new Point(j + LeftOffset, 12), new Point(j + LeftOffset, 20));
                    }
                }
            }
        }

        private void DrawSearchIntervals(DrawingContext dc)
        {
            for (var i = 0; i < _gs.SearchIntervalsLength; ++i)
            {
                var leftTop = new Point(_gs.SearchIntervals[i].Start, 0);
                var rightBottom = new Point(_gs.SearchIntervals[i].End, _mainGraphics.ActualHeight);
                dc.DrawRectangle(_gs.SearchIntervals[i].Selected
                        ? _selectedSearchAreaBrush
                        : (_gs.SearchIntervals[i].Hovered ? _hoveredSearchAreaBrush : _searchAreaBrush), null,
                    new Rect(leftTop, rightBottom));
                dc.DrawLine(_searchIntervalBorderPen, leftTop, new Point(leftTop.X, rightBottom.Y));
                dc.DrawLine(_searchIntervalBorderPen, new Point(rightBottom.X, leftTop.Y), rightBottom);
            }

            _manipulator.ReductionsManipulator.RedutionValueRects.Clear();
            if (_gs.ReductionValues.Count > 0)
            {
                for (var i = 0; i < _gs.ReductionValues.Count; ++i)
                {
                    var ft = new FormattedText(
                        _gs.ReductionForm.SelectedReduction.Name + ": " + _gs.ReductionValues[i].ToString("F2"),
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, ArialTypeface, 12, Brushes.Black);
                    var p1 = new Point(_gs.SelectedInterval.End + 2,
                        (i + 1) * ExpressionEditor.LineHeight - 2 * ft.Height - 3 - VerticalScroll);
                    dc.DrawText(ft, p1);

                    var ft2 = new FormattedText("▼", CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, ArialTypeface, 8, Brushes.Black);
                    dc.DrawText(ft2,
                        new Point(_gs.SelectedInterval.End + 3 + ft.Width,
                            (i + 1) * ExpressionEditor.LineHeight - 2 * ft.Height - VerticalScroll));

                    _manipulator.ReductionsManipulator.RedutionValueRects.Add(new Rect(
                        new Point(p1.X + LeftOffset, p1.Y + TopOffset),
                        new Point(p1.X + ft.Width + ft2.Width + 1 + LeftOffset, p1.Y + ft.Height + TopOffset)));
                }
            }
        }

        private void DrawInitialValueManipulator(DrawingContext dc, Point[] initialPoints, double?[] initialValues)
        {
            if (_manipulator.InitialValueManipulator.MouseX > LeftOffset)
            {
                return;
            }

            var bestDist = 0.0;
            var bestIndex = -1;

            if (_manipulator.InitialValueManipulator.DragStarted)
            {
                bestIndex = _manipulator.InitialValueManipulator.ManipulatedStatement;
            }
            else
            {
                for (var i = 0; i < initialPoints.Length; ++i)
                {
                    var y = initialPoints[i].Y + VerticalScroll;
                    if (y > ExpressionEditor.LineHeight * (i + 1))
                    {
                        y = ExpressionEditor.LineHeight * (i + 1);
                    }

                    if (y < ExpressionEditor.LineHeight * i)
                    {
                        y = ExpressionEditor.LineHeight * i;
                    }

                    var dx = initialPoints[i].X - _manipulator.InitialValueManipulator.MouseX + LeftOffset - 10;
                    var dy = y - (_manipulator.InitialValueManipulator.MouseY - TopOffset) - VerticalScroll;
                    var newDist = dx * dx + dy * dy;
                    if ((newDist < bestDist) || (bestIndex == -1))
                    {
                        bestDist = newDist;
                        bestIndex = i;
                    }
                }

                if (bestDist > 300)
                {
                    return;
                }
            }

            if (bestIndex == -1)
            {
                return;
            }

            if (initialValues[bestIndex] == null)
            {
                return;
            }

            _manipulator.InitialValueManipulator.ManipulatedStatement = bestIndex;

            var ft = new FormattedText(initialValues[bestIndex].Value.ToString("F2"), CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, ArialTypeface, 12, Brushes.Black);
            var yy = initialPoints[bestIndex].Y + VerticalScroll;
            if (yy > ExpressionEditor.LineHeight * (bestIndex + 1))
            {
                yy = ExpressionEditor.LineHeight * (bestIndex + 1);
            }

            if (yy < ExpressionEditor.LineHeight * bestIndex)
            {
                yy = ExpressionEditor.LineHeight * bestIndex;
            }

            var textPoint = new Point(LeftOffset - ft.Width - 2 - 3, yy - ft.Height / 2 - 2 - VerticalScroll);
            var rectPoint = new Point(textPoint.X - 3, textPoint.Y - 2);
            _manipulator.InitialValueManipulator.InitialValueManipulatorRect =
                new Rect(rectPoint, new Point(rectPoint.X + ft.Width + 6, rectPoint.Y + ft.Height + 4));
            dc.DrawRoundedRectangle(_initialValueManipulatorColor, null,
                _manipulator.InitialValueManipulator.InitialValueManipulatorRect, 3, 3);
            dc.DrawText(ft, textPoint);
        }
    }
}