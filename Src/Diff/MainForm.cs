using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Diff.Expressions;
using Diff.Manipulators;
using Diff.Reductions;

// TODO рисовать текст НАД интервалом

namespace Diff
{
    public partial class MainForm : Form
    {
        private readonly Drawer _drawer;
        private readonly GlobalScope _gs;
        private readonly MainGraphicOutput _mainGraphics = new MainGraphicOutput();

        public MainForm()
        {
            InitializeComponent();

            var reductionForm = new ReductionForm();
            reductionForm.ReductionChanged += ReductionFormOnReductionChanged;

            _gs = new GlobalScope(reductionForm);

#if DEBUG
            new DebugForm(expressionEditor1, searchTextBox, _gs).Show();
#endif

            _mainGraphics.SizeChanged += MainGraphicsOnSizeChanged;
            elementHost1.Child = _mainGraphics;

            var manipulator = new Manipulator(_mainGraphics, _gs);
            _drawer = new Drawer(_gs, _mainGraphics, manipulator);

            expressionEditor1.Scroll += ExpressionEditor1OnScroll;

            expressionEditor1.Text = "a[n] = 0";
        }

        private void ReductionFormOnReductionChanged(object sender, EventArgs eventArgs)
        {
            _gs.EvaluateReduction();
            _mainGraphics.InvalidateVisual();
        }

        private void ExpressionEditor1OnScroll(object sender, ScrollEventArgs scrollEventArgs)
        {
            _drawer.VerticalScroll = expressionEditor1.VerticalScroll.Value;
            _mainGraphics.InvalidateVisual();
        }

        private void MainGraphicsOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            _drawer.HostRect = new Rect(0, 0, _mainGraphics.ActualWidth, _mainGraphics.ActualHeight);
            GlobalScope.Iterations = (int) _mainGraphics.ActualWidth + 1 - Drawer.LeftOffset;
            RecalcRedraw();
        }

        private void expressionEditor1_TextChanged(object sender, EventArgs e)
        {
            expressionEditor1.RemoveAllMarkers();
            StatementsUpdated();
            RecalcRedraw();
        }

        private void StatementsUpdated()
        {
            var invalidStatements = _gs.UpdateAssignmentStatementsList(expressionEditor1.Text.Split('\n'));
            foreach (var m in invalidStatements)
            {
                expressionEditor1.AddMarker(m);
            }
        }

        private void Recalc()
        {
            var marker = _gs.Evaluate();
            if (marker != null)
            {
                if (marker.Line == -1)
                {
                    searchTextBox.ForeColor = Color.Red;
                }
                else
                {
                    expressionEditor1.AddMarker(marker);
                }
            }
        }

        private void RecalcRedraw()
        {
            Recalc();
            _mainGraphics.InvalidateVisual();
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_gs.Search(searchTextBox.Text) != null)
            {
                searchTextBox.ForeColor = Color.Red;
                return;
            }

            searchTextBox.ForeColor = Color.Black;
            RecalcRedraw();
        }
    }
}