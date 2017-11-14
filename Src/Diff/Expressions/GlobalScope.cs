using System.Collections.Generic;
using System.Drawing;
using Diff.Editor;
using Diff.Expressions.LowLevel;

namespace Diff.Expressions
{
    public class GlobalScope
    {
        public const int Iterations = 1000;
        public readonly List<AssignmentStatement> AssignmentStatements = new List<AssignmentStatement>();
        public readonly Dictionary<string, Variable> Globals = new Dictionary<string, Variable>();

        public LineMarker Evaluate()
        {
            for (var j = 0; j < Iterations; ++j)
            {
                for (var i = 0; i < AssignmentStatements.Count; ++i)
                {
                    AssignmentStatements[i].Locals["n"] = Variable.Const(j);
                    var errorMsg = AssignmentStatements[i].Evaluate(Globals);
                    if (errorMsg != null)
                    {
                        return new LineMarker {Color = Color.Red, Line = i + 1, Text = errorMsg};
                    }
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