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
                {"not", a => Variable.Const(!a.AsBool)},
                {"--", a => Variable.Const(-a.AsDouble)},
                {
                    "der", a =>
                    {
                        var n = a.IndexInArray;
                        return Variable.Const(a.Parent.NthItem(n + 1).AsDouble - a.Parent.NthItem(n).AsDouble);
                    }
                }
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
                },
                {
                    '?', (a, b) => Variable.Const(Math.Abs(a.AsDouble - b.AsDouble) < double.Epsilon)
                },
                {
                    '#', (a, b) => Variable.Const(Math.Abs(a.AsDouble - b.AsDouble) >= double.Epsilon)
                }
            };

        public static readonly Dictionary<char, int> Precedence = new Dictionary
            <char, int>
            {
                {
                    '=', 1
                },
                {
                    '?', 1
                },
                {
                    '#', 1
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