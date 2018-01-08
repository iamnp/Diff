using System.Windows.Input;
using Diff.Expressions;

namespace Diff.Manipulators
{
    internal class SearcnIntervalManipulator
    {
        private readonly GlobalScope _gs;
        private readonly MainGraphicOutput _mainGraphics;
        private double _dragShiftY;
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

        private void MainGraphicsOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
        }

        public void MainGraphicsOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var p = mouseEventArgs.GetPosition(_mainGraphics);
            MouseX = p.X;
            MouseY = p.Y;

            _mainGraphics.InvalidateVisual();
        }

        private void MainGraphicsOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
            {
                var p = mouseButtonEventArgs.GetPosition(_mainGraphics);
                p.Y -= Drawer.TopOffset;
                p.X -= Drawer.LeftOffset;

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