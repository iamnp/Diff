using System;
using System.Collections.Generic;

namespace Diff.Expressions.LowLevel
{
    public static class Calculator
    {
        public static readonly Dictionary<string, Func<Variable, Variable>> UnaryFunctions = new Dictionary
            <string, Func<Variable, Variable>>
            {
                {"sqrt", a => Variable.Const(Math.Sqrt(a.AsDouble))},
                {"--", a => Variable.Const(-a.AsDouble)}
            };

        public static readonly Dictionary<string, Func<Variable, Variable, Variable>> BinaryFunctions =
            new Dictionary
                <string, Func<Variable, Variable, Variable>>
                {
                    {"indexer", (a, b) => a.NthItem((int) b.AsDouble)}
                };

        public static readonly Dictionary<string, Func<Variable, Variable, Variable, Variable>> TrinaryFunctions =
            new Dictionary
                <string, Func<Variable, Variable, Variable, Variable>>
                {
                    {"if", (a, b, c) => a.AsBool ? b : c}
                };

        public static readonly Dictionary<char, Func<Variable, Variable, Variable>> Operators = new Dictionary
            <char, Func<Variable, Variable, Variable>>
            {
                {
                    '+', (a, b) => Variable.Const(a.AsDouble + b.AsDouble)
                },
                {
                    '-', (a, b) => Variable.Const(a.AsDouble - b.AsDouble)
                },
                {
                    '*', (a, b) => Variable.Const(a.AsDouble * b.AsDouble)
                },
                {
                    '/', (a, b) => Variable.Const(a.AsDouble / b.AsDouble)
                },
                {
                    '>', (a, b) => Variable.Const(a.AsDouble > b.AsDouble)
                },
                {
                    '<', (a, b) => Variable.Const(a.AsDouble < b.AsDouble)
                },
                {
                    '=', (a, b) => a.CopyValue(b)
                }
            };

        public static readonly Dictionary<char, int> Precedence = new Dictionary
            <char, int>
            {
                {
                    '=', 1
                },
                {
                    '>', 2
                },
                {
                    '<', 2
                },
                {
                    '+', 3
                },
                {
                    '-', 3
                },
                {
                    '*', 4
                },
                {
                    '/', 4
                }
            };

        private static readonly char[] RightAssocOperators = {'='};

        public static bool IsRightAssocOperator(char o)
        {
            for (var i = 0; i < RightAssocOperators.Length; ++i)
            {
                if (RightAssocOperators[i] == o)
                {
                    return true;
                }
            }

            return false;
        }
    }
}