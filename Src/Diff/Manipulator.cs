using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Diff.Editor;
using Diff.Expressions;

namespace Diff
{
    internal class Manipulator
    {
        private readonly GlobalScope _gs;
        private readonly MainGraphicOutput _mainGraphics;
        private Point _dragStart;
        public Rect InitialValueManipulatorRect;
        public int ManipulatedStatement;
        public double MouseX = -1;
        public double MouseY = -1;

        public Manipulator(MainGraphicOutput mainGraphic, GlobalScope gs)
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

        private void MainGraphicsOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.ChangedButton != MouseButton.Left)
            {
                return;
            }
            var p = mouseButtonEventArgs.GetPosition(_mainGraphics);
            if (InitialValueManipulatorRect.Contains(p))
            {
                DragStarted = true;
                _dragStart = p;
            }
        }

        private void MainGraphicsOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            MouseX = -1;
            MouseY = -1;
            _mainGraphics.InvalidateVisual();
        }

        private void MainGraphicsOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var p = mouseEventArgs.GetPosition(_mainGraphics);
            MouseX = p.X;
            MouseY = p.Y;

            if (DragStarted)
            {
                var linesAbove = (int) MouseY / ExpressionEditor.LineHeight;
                var v = (MouseY - linesAbove * ExpressionEditor.LineHeight) / ExpressionEditor.LineHeight;
                v = 1 - v;
                v *= 2;
                v -= 1;
                Debug.WriteLine(v);
                _gs.SetInitialValue(v, ManipulatedStatement);
            }
            _gs.Evaluate();
            _mainGraphics.InvalidateVisual();
        }
    }
}