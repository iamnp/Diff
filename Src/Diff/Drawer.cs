using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Diff
{
    internal static class Drawer
    {
        private static readonly List<Point> Points = new List<Point>();
        private static Point _firstPoint;
        private static bool _finished = true;
        public static readonly Brush WhiteBrush = new SolidColorBrush(Colors.White);

        public static readonly Pen BlackPen = new Pen
        {
            Thickness = 1,
            Brush = Brushes.Black
        };

        static Drawer()
        {
            BlackPen.Freeze();
            WhiteBrush.Freeze();
        }

        public static void DrawPath(this DrawingContext dc, Point p)
        {
            if (_finished)
            {
                _firstPoint = p;
                _finished = false;
            }
            else
            {
                Points.Add(p);
            }
        }

        public static void StopPath(this DrawingContext dc, Rect visible)
        {
            if (!_finished)
            {
                var geometry = new StreamGeometry();
                using (var ctx = geometry.Open())
                {
                    ctx.BeginFigure(_firstPoint, true, true);
                    if (Points.Count > 0)
                    {
                        Points.Add(visible.BottomRight);
                        Points.Add(visible.BottomLeft);
                        ctx.PolyLineTo(Points, true, false);
                    }
                }
                geometry.Freeze();
                var visibleRect = new RectangleGeometry(visible);
                visibleRect.Freeze();
                dc.PushClip(visibleRect);
                dc.DrawGeometry(Brushes.LightGray, BlackPen, geometry);
                dc.Pop();
            }
            Points.Clear();
            _finished = true;
        }
    }
}