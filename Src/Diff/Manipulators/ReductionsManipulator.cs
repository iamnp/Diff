using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Diff.Expressions;

namespace Diff.Manipulators
{
    internal class ReductionsManipulator
    {
        private readonly GlobalScope _gs;
        private readonly MainGraphicOutput _mainGraphics;
        public readonly List<Rect> RedutionValueRects = new List<Rect>();
        private bool _clicked;

        public ReductionsManipulator(MainGraphicOutput mainGraphic, GlobalScope gs)
        {
            _mainGraphics = mainGraphic;
            _gs = gs;

            _mainGraphics.MouseDown += MainGraphicsOnMouseDown;
            _mainGraphics.MouseUp += MainGraphicsOnMouseUp;
        }

        private void MainGraphicsOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (_clicked)
            {
                _gs.ReductionForm.Show();
                _gs.ReductionForm.MoveBelowCursor();
                _gs.ReductionForm.Focus();
            }
        }

        private void MainGraphicsOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _clicked = false;
            if (e.ChangedButton == MouseButton.Left)
            {
                var p = e.GetPosition(_mainGraphics);
                for (var i = 0; i < RedutionValueRects.Count; ++i)
                {
                    if (RedutionValueRects[i].Contains(p))
                    {
                        _clicked = true;
                        e.Handled = true;
                        break;
                    }
                }
            }
        }
    }
}