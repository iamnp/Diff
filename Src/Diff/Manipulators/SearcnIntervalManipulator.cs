using System.Windows.Input;
using Diff.Expressions;

namespace Diff.Manipulators
{
    internal class SearcnIntervalManipulator
    {
        private readonly GlobalScope _gs;
        private readonly MainGraphicOutput _mainGraphics;

        private bool _dragStarted;
        private GlobalScope.SearchInterval _hoveredInterval;
        private int _intervalStart;
        public double MouseX = -1;
        public double MouseY = -1;

        public SearcnIntervalManipulator(MainGraphicOutput mainGraphic, GlobalScope gs)
        {
            _mainGraphics = mainGraphic;
            _gs = gs;

            _mainGraphics.MouseMove += MainGraphicsOnMouseMove;
            _mainGraphics.MouseLeave += MainGraphicsOnMouseLeave;
            _mainGraphics.MouseDown += MainGraphicsOnMouseDown;
            _mainGraphics.MouseUp += MainGraphicsOnMouseUp;
        }

        private void MainGraphicsOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var p = mouseButtonEventArgs.GetPosition(_mainGraphics);
            p.Y -= Drawer.TopOffset;
            p.X -= Drawer.LeftOffset;

            if (mouseButtonEventArgs.ChangedButton == MouseButton.Right)
            {
                _dragStarted = false;
            }
        }

        public void MainGraphicsOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var p = mouseEventArgs.GetPosition(_mainGraphics);
            MouseX = p.X;
            MouseY = p.Y;
            p.Y -= Drawer.TopOffset;
            p.X -= Drawer.LeftOffset;

            if (mouseEventArgs.RightButton == MouseButtonState.Pressed)
            {
                if (!_dragStarted)
                {
                    _dragStarted = true;
                    _gs.ManualSearchInterval = new GlobalScope.SearchInterval
                    {
                        Start = _intervalStart,
                        End = _intervalStart,
                        Selected = true
                    };
                    _gs.SelectedInterval = _gs.ManualSearchInterval;
                    _gs.ClearSearchIntervals();
                    _gs.UpdateSearchIntervals();
                }

                if (p.X >= _intervalStart)
                {
                    _gs.ManualSearchInterval.Start = _intervalStart;
                    _gs.ManualSearchInterval.End = (int) p.X;
                }
                else
                {
                    _gs.ManualSearchInterval.End = _intervalStart;
                    _gs.ManualSearchInterval.Start = (int) p.X;
                }

                _gs.EvaluateReduction();
            }

            if ((mouseEventArgs.RightButton == MouseButtonState.Released) &&
                (mouseEventArgs.LeftButton == MouseButtonState.Released))
            {
                if (_hoveredInterval != null)
                {
                    _hoveredInterval.Hovered = false;
                    _hoveredInterval = null;
                }

                for (var i = 0; i < _gs.SearchIntervalsLength; ++i)
                {
                    if ((_gs.SearchIntervals[i].Start <= p.X) && (_gs.SearchIntervals[i].End >= p.X))
                    {
                        _hoveredInterval = _gs.SearchIntervals[i];
                        _hoveredInterval.Hovered = true;
                        break;
                    }
                }
            }

            _mainGraphics.InvalidateVisual();
        }

        private void MainGraphicsOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var p = mouseButtonEventArgs.GetPosition(_mainGraphics);
            p.Y -= Drawer.TopOffset;
            p.X -= Drawer.LeftOffset;

            if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
            {
                if (_gs.SelectedInterval != null)
                {
                    _gs.SelectedInterval.Selected = false;
                    _gs.SelectedInterval = null;
                }

                for (var i = 0; i < _gs.SearchIntervalsLength; ++i)
                {
                    if ((_gs.SearchIntervals[i].Start <= p.X) && (_gs.SearchIntervals[i].End >= p.X))
                    {
                        _gs.SelectedInterval = _gs.SearchIntervals[i];
                        _gs.SelectedInterval.Selected = true;
                        break;
                    }
                }
            }
            else if (mouseButtonEventArgs.ChangedButton == MouseButton.Right)
            {
                if (_gs.ManualSearchInterval != null)
                {
                    _gs.ManualSearchInterval.Selected = false;
                    _gs.SelectedInterval = null;
                }

                _gs.EvaluateReduction();

                _intervalStart = (int) p.X;

                _gs.ManualSearchInterval = null;
                _gs.ClearSearchIntervals();
                _gs.UpdateSearchIntervals();
            }

            _mainGraphics.InvalidateVisual();
        }

        private void MainGraphicsOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            MouseX = -1;
            MouseY = -1;
            _mainGraphics.InvalidateVisual();
        }
    }
}