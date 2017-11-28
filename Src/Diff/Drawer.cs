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

        private static StreamGeometry _geometry;
        private static StreamGeometryContext _ctx;

        static Drawer()
        {
            BlackPen.Freeze();
            WhiteBrush.Freeze();
        }

        public static void StartSession(this DrawingContext dc)
        {
            _geometry = new StreamGeometry();
            _ctx = _geometry.Open();
        }

        public static void FlushSession(this DrawingContext dc)
        {
            _ctx.Close();
            _geometry.Freeze();
            dc.DrawGeometry(null, BlackPen, _geometry);
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

        public static void StopPath(this DrawingContext dc)
        {
            if (!_finished)
            {
                _ctx.BeginFigure(_firstPoint, false, false);
                if (Points.Count > 0)
                {
                    _ctx.PolyLineTo(Points, true, false);
                }
            }
            Points.Clear();
            _finished = true;
        }
    }
}