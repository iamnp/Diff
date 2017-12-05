using System;
using System.Windows;
using System.Windows.Forms;
using Diff.Expressions;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

// === FEATURES ===
// TODO добавить ползунок для изменения начального значения

// === FIXES ===
// TODO посмотреть почему убывает последний график на тестах

namespace Diff
{
    public partial class MainForm : Form
    {
        private readonly Drawer _drawer;
        private readonly GlobalScope _gs = new GlobalScope();
        private readonly MainGraphicOutput _mainGraphics = new MainGraphicOutput();

        public MainForm()
        {
            InitializeComponent();

            _mainGraphics.MouseMove += MainGraphicsOnMouseMove;
            _mainGraphics.MouseLeave += MainGraphicsOnMouseLeave;
            _mainGraphics.SizeChanged += MainGraphicsOnSizeChanged;
            elementHost1.Child = _mainGraphics;
            _drawer = new Drawer(_gs, _mainGraphics);

            //VerticalScroll.SmallChange = LineHeight;
        }

        private void MainGraphicsOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            _drawer.HostRect = new Rect(0, 0, _mainGraphics.ActualWidth, _mainGraphics.ActualHeight);
            GlobalScope.Iterations = (int) _mainGraphics.ActualWidth + 1;
            RecalcNeeded();
            _mainGraphics.InvalidateVisual();
        }

        private void MainGraphicsOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            _drawer.MouseX = -1;
            _mainGraphics.InvalidateVisual();
        }

        private void MainGraphicsOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            _drawer.MouseX = mouseEventArgs.GetPosition(_mainGraphics).X;
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
            expressionEditor1.Text = "a = 1";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text =
                "a[n] = a[n-1]-0.05*b[n-1]\r\nb[n] = b[n-1]+0.05*a[n]\r\nc[n] = if(a[n] > 0, 1, -1)\r\nd[n] = d[n-1] + 0.03*c[n]";
        }
    }
}