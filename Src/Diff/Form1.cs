using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Diff.Expressions;
using Diff.Manipulators;

// === FEATURES ===
// TODO добавить редукции над интервалами (mean, max, min) с run-time compilation
// TODO добавить свои кастомные редукции
// TODO добавить подсказки при использовании

// === FIXES ===
// TODO починить поломку при резком увелечнии или уменьшении первого параметра

// === BACKLOG ===
// TODO хороший GUI
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

            _mainGraphics.SizeChanged += MainGraphicsOnSizeChanged;
            elementHost1.Child = _mainGraphics;

            var manipulator = new Manipulator(_mainGraphics, _gs);
            _drawer = new Drawer(_gs, _mainGraphics, manipulator);

            expressionEditor1.Scroll += ExpressionEditor1OnScroll;
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

        private void button1_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text = "a = 1.00";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text =
                "a[n+1] = a[n]-0.050*b[n]\r\nb[n+1] = b[n]+0.050*a[n+1]\r\nc[n+1] = if(a[n] > 0, 1, -1)\r\nd[n+1] = d[n] + 0.030*c[n]";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "a[n+1] > a[n]";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            expressionEditor1.Text =
                "a = -0.9\r\nb = -0.8\r\nc = -0.7\r\nd = -0.6\r\ne = -0.5\r\nf = -0.4\r\ng = -0.3\r\nh = -0.2\r\ni = -0.1\r\nj = 0\r\nk = 0.1\r\nl = 0.2\r\nm = 0.3\r\nn = 0.4\r\no = 0.5\r\np = 0.6\r\nq = 0.7\r\nr = 0.8\r\ns = 0.9";
        }
    }
}