using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Diff.Expressions;
using Diff.Manipulators;

// === FEATURES ===
// TODO сделать пользовательские интервалы
// TODO добавить редукции над интервалами (mean, max, min)
// TODO добавить подсказки при использовании

// === FIXES ===
// TODO починить поломку при резком увелечнии или уменьшении первого параметра

// === BACKLOG ===
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

            //VerticalScroll.SmallChange = LineHeight;
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
    }
}