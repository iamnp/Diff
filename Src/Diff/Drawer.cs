﻿using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Diff.Editor;
using Diff.Expressions;

namespace Diff
{
    internal class Drawer
    {
        public const int LeftOffset = 50;

        private readonly Pen _blackPen = new Pen
        {
            Thickness = 1,
            Brush = Brushes.Black
        };

        private readonly GlobalScope _gs;
        private readonly MainGraphicOutput _mainGraphics;
        private readonly Manipulator _manipulator;

        private readonly List<Point> _points = new List<Point>();
        private readonly Brush _whiteBrush = new SolidColorBrush(Colors.White);
        private bool _finished = true;
        private Point _firstPoint;
        public Rect HostRect;


        public Drawer(GlobalScope gs, MainGraphicOutput mainGraphics, Manipulator manipulator)
        {
            _gs = gs;
            _mainGraphics = mainGraphics;
            _manipulator = manipulator;

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
            dc.PushTransform(new TranslateTransform(LeftOffset, 0));

            var initialPoints = new Point[_gs.AssignmentStatements.Count];
            var initialValues = new double?[_gs.AssignmentStatements.Count];

            for (var i = 0; i < _gs.AssignmentStatements.Count; ++i)
            {
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
                                if ((int) _manipulator.MouseX - LeftOffset == j)
                                {
                                    value = yPoint;
                                }
                                var v = yPoint;
                                yPoint *= -ExpressionEditor.LineHeight / 2.0;
                                yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2;
                                var p = new Point(j, yPoint);
                                DrawPath(p);
                                if (!set)
                                {
                                    set = true;
                                    initialValues[i] = v;
                                    initialPoints[i] = p;
                                }
                            }
                        }
                        else if (_gs.AssignmentStatements[i].Assignee.IsDouble)
                        {
                            var yPoint = _gs.AssignmentStatements[i].Assignee.AsDouble;
                            if ((int) _manipulator.MouseX - LeftOffset == j)
                            {
                                value = yPoint;
                            }
                            var v = yPoint;
                            yPoint *= -ExpressionEditor.LineHeight / 2.0;
                            yPoint += i * ExpressionEditor.LineHeight + ExpressionEditor.LineHeight / 2;
                            var p = new Point(j, yPoint);
                            DrawPath(p);
                            if (!set)
                            {
                                set = true;
                                initialValues[i] = v;
                                initialPoints[i] = p;
                            }
                        }
                    }
                }
                StopPath(dc, new Rect(0, i * ExpressionEditor.LineHeight, _mainGraphics.ActualWidth,
                    ExpressionEditor.LineHeight));
                if ((value != null) && (_manipulator.MouseX > LeftOffset))
                {
                    var ft = new FormattedText(value.Value.ToString("F2"), CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Black);
                    dc.DrawText(ft,
                        new Point(_manipulator.MouseX + 4 - LeftOffset,
                            (i + 1) * ExpressionEditor.LineHeight - ft.Height - 3));
                }
                dc.DrawLine(_blackPen, new Point(0, (float) (i + 1) * ExpressionEditor.LineHeight), new Point(
                    _mainGraphics.ActualWidth,
                    (float) (i + 1) * ExpressionEditor.LineHeight));
            }

            if (_manipulator.MouseX > LeftOffset)
            {
                dc.DrawLine(_blackPen, new Point(_manipulator.MouseX - LeftOffset, 0),
                    new Point(_manipulator.MouseX - LeftOffset, _mainGraphics.ActualHeight));
            }

            dc.Pop();

            DrawInitialValueManipulator(dc, initialPoints, initialValues);
        }

        private void DrawInitialValueManipulator(DrawingContext dc, Point[] initialPoints, double?[] initialValues)
        {
            if (_manipulator.MouseX > LeftOffset)
            {
                return;
            }

            var bestDist = 0.0;
            var bestIndex = -1;

            if (_manipulator.DragStarted)
            {
                bestIndex = _manipulator.ManipulatedStatement;
            }
            else
            {
                for (var i = 0; i < initialPoints.Length; ++i)
                {
                    var dx = initialPoints[i].X - _manipulator.MouseX + LeftOffset;
                    var dy = initialPoints[i].Y - _manipulator.MouseY;
                    var newDist = dx * dx + dy * dy;
                    if ((newDist < bestDist) || (bestIndex == -1))
                    {
                        bestDist = newDist;
                        bestIndex = i;
                    }
                }

                if (bestDist > 400)
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

            _manipulator.ManipulatedStatement = bestIndex;

            var ft = new FormattedText(initialValues[bestIndex].Value.ToString("F2"), CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Black);
            var p1 = new Point(LeftOffset - ft.Width - 2, initialPoints[bestIndex].Y - ft.Height / 2);
            _manipulator.InitialValueManipulatorRect = new Rect(p1, new Point(p1.X + ft.Width, p1.Y + ft.Height));
            dc.DrawRoundedRectangle(Brushes.Aqua, null, _manipulator.InitialValueManipulatorRect, 3, 3);
            dc.DrawText(ft, p1);
        }
    }
}