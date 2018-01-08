using System.Windows;
using System.Windows.Input;
using Diff.Editor;
using Diff.Expressions;

namespace Diff.Manipulators
{
    internal class InitialValueManipulator
    {
        private readonly GlobalScope _gs;
        private readonly MainGraphicOutput _mainGraphics;
        private double _dragShiftY;
        public Rect InitialValueManipulatorRect;
        public int ManipulatedStatement;
        public double MouseX = -1;
        public double MouseY = -1;

        public InitialValueManipulator(MainGraphicOutput mainGraphic, GlobalScope gs)
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
            DragStarted = false;
        }

        public void MainGraphicsOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var p = mouseEventArgs.GetPosition(_mainGraphics);
            MouseX = p.X;
            MouseY = p.Y;

            if (MouseX > Drawer.LeftOffset)
            {
                DragStarted = false;
            }

            if (DragStarted)
            {
                var v = (MouseY - Drawer.TopOffset - _dragShiftY - ManipulatedStatement * ExpressionEditor.LineHeight) /
                        ExpressionEditor.LineHeight;
                v = 1 - v;
                v *= 2;
                v -= 1;
                _gs.SetInitialValue(v, ManipulatedStatement);
                _gs.Evaluate();
                _mainGraphics.InvalidateVisual();
            }
        }

        private void MainGraphicsOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
            {
                var p = mouseButtonEventArgs.GetPosition(_mainGraphics);
                p.Y -= Drawer.TopOffset;
                if (InitialValueManipulatorRect.Contains(p))
                {
                    DragStarted = true;
                    _dragShiftY = p.Y - (InitialValueManipulatorRect.Top + InitialValueManipulatorRect.Bottom) / 2.0;
                }
            }
        }

        private void MainGraphicsOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            MouseX = -1;
            MouseY = -1;
            DragStarted = false;
            _mainGraphics.InvalidateVisual();
        }
    }
}