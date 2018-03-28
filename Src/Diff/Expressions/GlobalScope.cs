using System.Collections.Generic;
using System.Drawing;
using Diff.Editor;
using Diff.Expressions.LowLevel;
using Diff.Reductions;

namespace Diff.Expressions
{
    public class GlobalScope
    {
        private const string SearchVar = "searched";
        public const string NVar = "n";
        public static int Iterations = 300;
        public readonly List<AssignmentStatement> AssignmentStatements = new List<AssignmentStatement>();
        public readonly Dictionary<string, Variable> Globals = new Dictionary<string, Variable>();
        public readonly ReductionForm ReductionForm;
        public readonly List<double> ReductionValues = new List<double>();
        public readonly List<SearchInterval> SearchIntervals = new List<SearchInterval>();
        private AssignmentStatement _searchStatement;
        public SearchInterval ManualSearchInterval;

        public GlobalScope(ReductionForm reductionForm = null)
        {
            ReductionForm = reductionForm;
        }

        public int SearchIntervalsLength { get; private set; }
        public SearchInterval SelectedInterval { get; set; }

        public void ClearSearchIntervals()
        {
            SearchIntervalsLength = 0;
        }

        public void AddSearchInterval(int start, int end)
        {
            if (SearchIntervals.Count < SearchIntervalsLength + 1)
            {
                SearchIntervals.Add(new SearchInterval {Start = start, End = end});
            }
            else
            {
                SearchIntervals[SearchIntervalsLength].Start = start;
                SearchIntervals[SearchIntervalsLength].End = end;
            }

            SearchIntervalsLength += 1;
        }

        public void AddSearchInterval(SearchInterval si)
        {
            if (SearchIntervals.Count < SearchIntervalsLength + 1)
            {
                SearchIntervals.Add(si);
            }
            else
            {
                SearchIntervals[SearchIntervalsLength] = si;
            }

            SearchIntervalsLength += 1;
        }

        public string Search(string expr)
        {
            if (expr == "")
            {
                _searchStatement = null;
                return null;
            }

            if (_searchStatement == null)
            {
                _searchStatement = new AssignmentStatement();
            }

            return _searchStatement.SetExprString(SearchVar + "[" + NVar + "] = (" + expr + ")");
        }

        public bool IsIterationFound(int n)
        {
            Variable v;
            if (Globals.TryGetValue(SearchVar, out v) && v.NthItem(n).IsBool)
            {
                return v.NthItem(n).AsBool;
            }

            return false;
        }

        public void SetInitialValue(double v, int index)
        {
            var var = AssignmentStatements[index].Assignee;
            if (var.Parent != null)
            {
                var = var.Parent;
            }

            var.SetDoubleValue(v);
            var.NthItem(0).SetDoubleValue(v);
        }

        public LineMarker Evaluate()
        {
            LineMarker lineMarker = null;

            for (var j = 0; j < Iterations; ++j)
            {
                for (var i = 0; i < AssignmentStatements.Count; ++i)
                {
                    AssignmentStatements[i].Locals[NVar] = Variable.Const(j);
                    var errorMsg = AssignmentStatements[i].Evaluate(Globals);
                    if (errorMsg != null)
                    {
                        if (lineMarker == null)
                        {
                            lineMarker = new LineMarker {Color = Color.Red, Line = i + 1, Text = errorMsg};
                        }
                    }
                }
            }


            for (var j = 0; j < Iterations; ++j)
            {
                if (_searchStatement != null)
                {
                    _searchStatement.Locals[NVar] = Variable.Const(j);
                    var errorMsgg = _searchStatement.Evaluate(Globals);
                    if (errorMsgg != null)
                    {
                        if (lineMarker == null)
                        {
                            lineMarker = new LineMarker {Color = Color.Red, Line = -1, Text = errorMsgg};
                        }
                    }
                }
                else if (Globals.ContainsKey(SearchVar))
                {
                    Globals[SearchVar].NthItem(j).SetBoolValue(false);
                }
            }

            ClearSearchIntervals();

            UpdateSearchIntervals();

            EvaluateReduction();

            return null;
        }

        public void EvaluateReduction()
        {
            ReductionValues.Clear();
            if ((SelectedInterval != null) && (ReductionForm.SelectedReduction != null))
            {
                for (var i = 0; i < AssignmentStatements.Count; ++i)
                {
                    ReductionValues.Add(
                        ReductionForm.SelectedReduction.Perform(SelectedInterval, AssignmentStatements[i]));
                }
            }
        }

        public void UpdateSearchIntervals()
        {
            if (ManualSearchInterval != null)
            {
                AddSearchInterval(ManualSearchInterval);
                return;
            }

            var start = -1;
            for (var j = 0; j < Iterations; ++j)
            {
                if (IsIterationFound(j))
                {
                    if (start == -1)
                    {
                        start = j;
                    }
                }
                else
                {
                    if (start != -1)
                    {
                        AddSearchInterval(start, j - 1);
                        start = -1;
                    }
                }
            }

            if (start != -1)
            {
                AddSearchInterval(start, Iterations);
            }
        }

        public List<LineMarker> UpdateAssignmentStatementsList(string[] statements)
        {
            while (AssignmentStatements.Count > statements.Length)
            {
                AssignmentStatements.RemoveAt(AssignmentStatements.Count - 1);
            }

            while (AssignmentStatements.Count < statements.Length)
            {
                AssignmentStatements.Add(new AssignmentStatement());
            }

            var invalidStatements = new List<LineMarker>();

            for (var i = 0; i < AssignmentStatements.Count; ++i)
            {
                var errorMsg = AssignmentStatements[i].SetExprString(statements[i]);
                if (errorMsg != null)
                {
                    invalidStatements.Add(new LineMarker {Color = Color.Red, Line = i + 1, Text = errorMsg});
                }
            }

            return invalidStatements;
        }

        public class SearchInterval
        {
            public int End;
            public bool Hovered;
            public bool Selected;
            public int Start;
        }
    }
}