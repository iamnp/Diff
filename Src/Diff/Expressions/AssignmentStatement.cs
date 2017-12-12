using System;
using System.Collections.Generic;
using Diff.Expressions.LowLevel;

namespace Diff.Expressions
{
    public class AssignmentStatement
    {
        public readonly Dictionary<string, Variable> Locals = new Dictionary<string, Variable>();
        private ParsedExpression _parsedExpression;

        public AssignmentStatement()
        {
            Locals.Add(GlobalScope.NVar, Variable.Const(0.0));
        }

        public Variable Assignee { get; private set; }

        public string Evaluate(Dictionary<string, Variable> globals)
        {
            try
            {
                Assignee = _parsedExpression?.Evaluate(globals, Locals);
            }
            catch (Exception ex)
            {
                Assignee = null;
                return ex.Message;
            }
            return null;
        }

        public string SetExprString(string exprString)
        {
            if ((_parsedExpression == null) || (_parsedExpression.AsString != exprString))
            {
                try
                {
                    _parsedExpression = ParsedExpression.Parse(exprString);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return null;
        }
    }
}