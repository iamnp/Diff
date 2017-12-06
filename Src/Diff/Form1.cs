using System;
using System.Windows;
using System.Windows.Forms;
using Diff.Expressions;

// === FEATURES ===


// === FIXES ===
// TODO make use of _dragStart in Manipulator (shift scroller a little bit)
// TODO Master scroller behaviaour: can extend beyond 1.0 and -1.0, but the scroller position is restricted to the line width range

// TODO посмотреть почему убывает последний график на тестах

namespace Diff
{
    public partial class MainForm : Form
    {
        private readonly Drawer _drawer;
        private readonly GlobalScope _gs = new GlobalScope();
        private readonly MainGraphicOutput _mainGraphics = new MainGraphicOutput();

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Manipulator _manipulator;

        public MainForm()
        {
            InitializeComponent();

            _mainGraphics.SizeChanged += MainGraphicsOnSizeChanged;
            elementHost1.Child = _mainGraphics;

            _manipulator = new Manipulator(_mainGraphics, _gs);
            _drawer = new Drawer(_gs, _mainGraphics, _manipulator);

            //VerticalScroll.SmallChange = LineHeight;
        }

        private void MainGraphicsOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            _drawer.HostRect = new Rect(0, 0, _mainGraphics.ActualWidth, _mainGraphics.ActualHeight);
            GlobalScope.Iterations = (int) _mainGraphics.ActualWidth + 1 - Drawer.LeftOffset;
            RecalcNeeded();
            _mainGraphics.InvalidateVisual();
        }

        private void expressionEditor1_TextChanged(object sender, EventArgs e)
        {
            expressionEditor1.RemoveAllMarkers();
            StatementsUpdated();
            RecalcNeeded();
            _mainGraphics.InvalidateVisual();
        }

        private void StatementsUpdated()
        {
            var invalidStatements = _gs.UpdateAssignmentStatementsList(expressionEditor1.Text.Split('\n'));
            foreach (var m in invalidStatements)
            {
                expressionEditor1.AddMarker(m);
            }
        }

        private void RecalcNeeded()
        {
            var marker = _gs.Evaluate();
            if (marker != null)
            {
                expressionEditor1.AddMarker(marker);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text = "a = 1.00";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text =
                "a[n] = a[n-1]-0.05*b[n-1]\r\nb[n] = b[n-1]+0.05*a[n]\r\nc[n] = if(a[n] > 0, 1, -1)\r\nd[n] = d[n-1] + 0.03*c[n]";
        }
    }
}