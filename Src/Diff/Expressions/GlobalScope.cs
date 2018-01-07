using System.Collections.Generic;
using System.Drawing;
using Diff.Editor;
using Diff.Expressions.LowLevel;

namespace Diff.Expressions
{
    public class GlobalScope
    {
        private const string SearchVar = "searched";
        public const string NVar = "n";
        public static int Iterations = 300;
        private readonly AssignmentStatement _searchStatement = new AssignmentStatement();
        public readonly List<AssignmentStatement> AssignmentStatements = new List<AssignmentStatement>();
        public readonly Dictionary<string, Variable> Globals = new Dictionary<string, Variable>();

        public string Search(string expr)
        {
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
        }

        public LineMarker Evaluate()
        {
            for (var j = 0; j < Iterations; ++j)
            {
                for (var i = 0; i < AssignmentStatements.Count; ++i)
                {
                    AssignmentStatements[i].Locals[NVar] = Variable.Const(j);
                    var errorMsg = AssignmentStatements[i].Evaluate(Globals);
                    if (errorMsg != null)
                    {
                        return new LineMarker {Color = Color.Red, Line = i + 1, Text = errorMsg};
                    }
                }

                _searchStatement.Locals[NVar] = Variable.Const(j);
                var errorMsgg = _searchStatement.Evaluate(Globals);
                if (errorMsgg != null)
                {
                    return new LineMarker {Color = Color.Red, Line = -1, Text = errorMsgg};
                }
            }

            return null;
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
    }
}