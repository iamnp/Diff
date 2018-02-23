using System.Windows.Input;
using Diff.Expressions;

namespace Diff.Manipulators
{
    internal class SearcnIntervalManipulator
    {
        private readonly GlobalScope _gs;
        private readonly MainGraphicOutput _mainGraphics;
        private GlobalScope.SearchInterval _hoveredInterval;
        private GlobalScope.SearchInterval _selectedInterval;
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

        public bool DragStarted { get; private set; }

        private void MainGraphicsOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var p = mouseButtonEventArgs.GetPosition(_mainGraphics);
            p.Y -= Drawer.TopOffset;
            p.X -= Drawer.LeftOffset;

            if (mouseButtonEventArgs.ChangedButton == MouseButton.Right)
            {
                if (DragStarted)
                {
                    _gs.ManualSearchInterval.End = (int) p.X;
                    DragStarted = false;
                }
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
                if (!DragStarted)
                {
                    DragStarted = true;
                    _gs.ManualSearchInterval = new GlobalScope.SearchInterval
                    {
                        Start = (int) p.X,
                        End = (int) p.X,
                        Selected = true
                    };
                    _selectedInterval = _gs.ManualSearchInterval;
                    _gs.ClearSearchIntervals();
                    _gs.UpdateSearchIntervals();
                }

                _gs.ManualSearchInterval.End = (int) p.X;
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
                if (_selectedInterval != null)
                {
                    _selectedInterval.Selected = false;
                    _selectedInterval = null;
                }

                for (var i = 0; i < _gs.SearchIntervalsLength; ++i)
                {
                    if ((_gs.SearchIntervals[i].Start <= p.X) && (_gs.SearchIntervals[i].End >= p.X))
                    {
                        _selectedInterval = _gs.SearchIntervals[i];
                        _selectedInterval.Selected = true;
                        break;
                    }
                }
            }
            else if (mouseButtonEventArgs.ChangedButton == MouseButton.Right)
            {
                if (_gs.ManualSearchInterval != null)
                {
                    _gs.ManualSearchInterval.Selected = false;
                }

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