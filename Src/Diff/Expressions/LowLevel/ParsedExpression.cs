using System;
using System.Collections.Generic;

namespace Diff.Expressions.LowLevel
{
    public class ParsedExpression
    {
        private const int VariableId = 1;
        private const int FunctionId = 2;
        private const int OperatorId = 3;
        private const int LiteralId = 4;

        private readonly List<string> _functions;
        private readonly List<double> _literals;
        private readonly List<char> _operators;

        private readonly LinkedList<Tuple<int, int>> _outputQueue;
        private readonly List<string> _variables;
        public readonly string AsString;

        private ParsedExpression(List<string> variables, List<double> literals, List<string> functions,
            List<char> operators,
            LinkedList<Tuple<int, int>> outputQueue, string asString)
        {
            _variables = variables;
            _literals = literals;
            _functions = functions;
            _operators = operators;
            _outputQueue = outputQueue;
            AsString = asString;
        }

        public static ParsedExpression Parse(string exprString)
        {
            var variables = new List<string>();
            var literals = new List<double>();
            var functions = new List<string>();
            var operators = new List<char>();

            var outputQueue = new LinkedList<Tuple<int, int>>();
            var operatorStack = new Stack<Tuple<int, int>>();

            var firstInExpr = false;
            var afterOp = false;
            var afterLeftBrace = false;
            var afterComma = false;

            unsafe
            {
                fixed (char* k = exprString)
                {
                    for (var p = k; *p != '\0'; ++p)
                    {
                        if (char.IsDigit(*p)) // TOKEN: number
                        {
                            firstInExpr = true;
                            afterOp = false;
                            afterLeftBrace = false;
                            afterComma = false;
                            var start = p;
                            while ((*p != '\0') &&
                                   (char.IsDigit(*p) ||
                                    ((*p == '.') && (*(p + 1) != '\0') &&
                                     char.IsDigit(*(p + 1)))))
                            {
                                ++p;
                            }

                            --p;
                            literals.Add(double.Parse(MakeSubstring(start, p).Replace(".", ",")));
                            outputQueue.AddLast(new Tuple<int, int>(LiteralId, literals.Count - 1));
                        }
                        else if (*p == ',') // TOKEN: function arg delimeter
                        {
                            while ((operatorStack.Count > 0)
                                   && !((operatorStack.Peek().Item1 == OperatorId) &&
                                        (operators[operatorStack.Peek().Item2] == '(')))
                            {
                                outputQueue.AddLast(operatorStack.Pop());
                            }

                            if (operatorStack.Count == 0)
                            {
                                throw new Exception("Invalid expr!");
                            }

                            afterComma = true;
                        }
                        else if (char.IsLetter(*p)) // TOKEN: function or variable
                        {
                            firstInExpr = true;
                            afterOp = false;
                            afterLeftBrace = false;
                            afterComma = false;
                            var start = p;
                            while ((*p != '\0') &&
                                   (char.IsLetter(*p) || char.IsDigit(*p) ||
                                    ((*p == '.') && (*(p + 1) != '\0') &&
                                     (char.IsLetter(*(p + 1)) || char.IsDigit(*(p + 1))))))
                            {
                                ++p;
                            }

                            --p;
                            var funcOrVar = MakeSubstring(start, p);
                            if (Calculator.UnaryFunctions.ContainsKey(funcOrVar)
                                || Calculator.BinaryFunctions.ContainsKey(funcOrVar)
                                || Calculator.TrinaryFunctions.ContainsKey(funcOrVar))
                            {
                                // TOKEN: function
                                functions.Add(funcOrVar);
                                operatorStack.Push(new Tuple<int, int>(FunctionId, functions.Count - 1));
                            }
                            else // TOKEN: variable
                            {
                                variables.Add(funcOrVar);
                                outputQueue.AddLast(new Tuple<int, int>(VariableId, variables.Count - 1));
                            }
                        }
                        else if (Calculator.Operators.ContainsKey(*p)) // TOKEN: one-char operator
                        {
                            var o1 = *p;
                            // if minus is unary then threat it as a function
                            if ((o1 == '-') && (!firstInExpr || afterOp || afterLeftBrace || afterComma))
                            {
                                functions.Add("--");
                                operatorStack.Push(new Tuple<int, int>(FunctionId, functions.Count - 1));
                            }
                            // minus is binary operator
                            else
                            {
                                while ((operatorStack.Count > 0) && (operatorStack.Peek().Item1 == OperatorId) &&
                                       (operators[operatorStack.Peek().Item2] != '(') &&
                                       ((Calculator.IsRightAssocOperator(o1) &&
                                         (Calculator.Precedence[o1] <=
                                          Calculator.Precedence[operators[operatorStack.Peek().Item2]]))
                                        || (!Calculator.IsRightAssocOperator(o1) &&
                                            (Calculator.Precedence[o1] <
                                             Calculator.Precedence[operators[operatorStack.Peek().Item2]]))))
                                {
                                    outputQueue.AddLast(operatorStack.Pop());
                                }

                                operators.Add(o1);
                                operatorStack.Push(new Tuple<int, int>(OperatorId, operators.Count - 1));
                            }

                            afterOp = true;
                        }
                        else if ((*p == '(') || (*p == '['))
                        {
                            if (*p == '[')
                            {
                                functions.Add("indexer");
                                operatorStack.Push(new Tuple<int, int>(FunctionId, functions.Count - 1));
                            }

                            afterLeftBrace = true;
                            operators.Add('(');
                            operatorStack.Push(new Tuple<int, int>(OperatorId, operators.Count - 1));
                        }
                        else if ((*p == ')') || (*p == ']'))
                        {
                            firstInExpr = true;
                            afterOp = false;
                            afterLeftBrace = false;
                            afterComma = false;
                            while ((operatorStack.Count > 0) &&
                                   !((operatorStack.Peek().Item1 == OperatorId) &&
                                     (operators[operatorStack.Peek().Item2] == '(')))
                            {
                                outputQueue.AddLast(operatorStack.Pop());
                            }

                            operatorStack.Pop();
                            if ((operatorStack.Count > 0) && (operatorStack.Peek().Item1 == FunctionId))
                            {
                                outputQueue.AddLast(operatorStack.Pop());
                            }
                        }
                    }
                }
            }

            while (operatorStack.Count > 0)
            {
                if ((operatorStack.Peek().Item1 == OperatorId) && (operators[operatorStack.Peek().Item2] == '('))
                {
                    throw new Exception("Invalid expr!");
                }

                outputQueue.AddLast(operatorStack.Pop());
            }

            return new ParsedExpression(variables, literals, functions, operators, outputQueue, exprString);
        }

        public Variable Evaluate(Dictionary<string, Variable> globals, Dictionary<string, Variable> locals)
        {
            var varStack = new Stack<Variable>();

            for (var node = _outputQueue.First; node != null; node = node.Next)
            {
                var token = node.Value;
                if (token.Item1 == VariableId)
                {
                    if ((locals != null) && locals.ContainsKey(_variables[token.Item2]))
                    {
                        varStack.Push(locals[_variables[token.Item2]]);
                    }
                    else if (globals != null)
                    {
                        if (!globals.ContainsKey(_variables[token.Item2]))
                        {
                            globals.Add(_variables[token.Item2], Variable.Empty(_variables[token.Item2]));
                        }

                        varStack.Push(globals[_variables[token.Item2]]);
                    }
                }
                else if (token.Item1 == LiteralId)
                {
                    varStack.Push(Variable.Const(_literals[token.Item2]));
                }
                else if (token.Item1 == OperatorId)
                {
                    if (varStack.Count < 2)
                    {
                        throw new Exception("Invalid expr!");
                    }

                    var v1 = varStack.Pop();
                    var v2 = varStack.Pop();
                    varStack.Push(Calculator.Operators[_operators[token.Item2]](v2, v1));
                }
                // it is a function
                else
                {
                    var funcName = _functions[token.Item2];
                    if (Calculator.UnaryFunctions.ContainsKey(funcName))
                    {
                        if (varStack.Count < 1)
                        {
                            throw new Exception("Invalid expr!");
                        }

                        varStack.Push(Calculator.UnaryFunctions[funcName](varStack.Pop()));
                    }
                    else if (Calculator.BinaryFunctions.ContainsKey(funcName))
                    {
                        if (varStack.Count < 2)
                        {
                            throw new Exception("Invalid expr!");
                        }

                        var v1 = varStack.Pop();
                        var v2 = varStack.Pop();
                        varStack.Push(Calculator.BinaryFunctions[funcName](v2, v1));
                    }
                    else if (Calculator.TrinaryFunctions.ContainsKey(funcName))
                    {
                        if (varStack.Count < 3)
                        {
                            throw new Exception("Invalid expr!");
                        }

                        var v1 = varStack.Pop();
                        var v2 = varStack.Pop();
                        var v3 = varStack.Pop();
                        varStack.Push(Calculator.TrinaryFunctions[funcName](v3, v2, v1));
                    }
                }
            }

            if (varStack.Count > 1)
            {
                throw new Exception("Invalid expr!");
            }

            return varStack.Peek();
        }

        private static unsafe string MakeSubstring(char* p1, char* p2)
        {
            return new string(p1, 0, (int) (p2 - p1) + 1);
        }
    }
}