using System.Collections.Generic;
using Diff.Expressions.LowLevel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiffTest
{
    [TestClass]
    public class ExpressionTest
    {
        [TestMethod]
        public void TestSimpleExpressions()
        {
            var vars = new Dictionary<string, Variable>();

            Assert.AreEqual(ParsedExpression.Parse("2+2").Evaluate(vars, vars).AsDouble, 4);
            Assert.AreEqual(ParsedExpression.Parse("2+2*2").Evaluate(vars, vars).AsDouble, 6);
            Assert.AreEqual(ParsedExpression.Parse("2*2+2").Evaluate(vars, vars).AsDouble, 6);
            Assert.AreEqual(ParsedExpression.Parse("2+(2*2)").Evaluate(vars, vars).AsDouble, 6);
            Assert.AreEqual(ParsedExpression.Parse("2*(2+2)").Evaluate(vars, vars).AsDouble, 8);
            Assert.AreEqual(ParsedExpression.Parse("(2+2)*2").Evaluate(vars, vars).AsDouble, 8);
            Assert.AreEqual(ParsedExpression.Parse("(2*2)+2").Evaluate(vars, vars).AsDouble, 6);
            Assert.AreEqual(ParsedExpression.Parse("-2").Evaluate(vars, vars).AsDouble, -2);
            Assert.AreEqual(ParsedExpression.Parse("sqrt(4)").Evaluate(vars, vars).AsDouble, 2);
            Assert.AreEqual(ParsedExpression.Parse("sqrt(2+2)").Evaluate(vars, vars).AsDouble, 2);
            Assert.AreEqual(ParsedExpression.Parse("sqrt((2+2)*4)").Evaluate(vars, vars).AsDouble, 4);

            Assert.AreEqual(ParsedExpression.Parse("2 > 1").Evaluate(vars, vars).AsBool, true);
            Assert.AreEqual(ParsedExpression.Parse("2 < 1").Evaluate(vars, vars).AsBool, false);

            Assert.AreEqual(ParsedExpression.Parse("2+1 < 1-1").Evaluate(vars, vars).AsBool, false);

            Assert.AreEqual(ParsedExpression.Parse("if(101 > 5, 17, 38)").Evaluate(vars, vars).AsDouble, 17);
            Assert.AreEqual(ParsedExpression.Parse("if(2 < 1, 1, 0)").Evaluate(vars, vars).AsDouble, 0);
            Assert.AreEqual(ParsedExpression.Parse("if(2 < 1, 1, -1)").Evaluate(vars, vars).AsDouble, -1);
        }

        [TestMethod]
        public void TestReEvaluate()
        {
            var vars = new Dictionary<string, Variable>();

            var pe = ParsedExpression.Parse("2+2");
            Assert.AreEqual(pe.Evaluate(vars, vars).AsDouble, 4);
            Assert.AreEqual(pe.Evaluate(vars, vars).AsDouble, 4);
            Assert.AreEqual(pe.Evaluate(vars, vars).AsDouble, 4);
        }

        [TestMethod]
        public void TestSimpleVariables()
        {
            var globals = new Dictionary<string, Variable>
            {
                {"a", Variable.Const(2)}
            };
            var locals = new Dictionary<string, Variable>();

            Assert.AreEqual(ParsedExpression.Parse("2+a").Evaluate(globals, locals).AsDouble, 4);
        }

        [TestMethod]
        public void TestVariablesPriority()
        {
            var globals = new Dictionary<string, Variable>
            {
                {"a", Variable.Const(2)}
            };
            var locals = new Dictionary<string, Variable>
            {
                {"a", Variable.Const(3)}
            };

            Assert.AreEqual(ParsedExpression.Parse("2+a").Evaluate(globals, locals).AsDouble, 5);
        }

        [TestMethod]
        public void TestIndexerAndArray()
        {
            var globals = new Dictionary<string, Variable>
            {
                {"a", Variable.Const(2)},
                {"b", Variable.Const(3)}
            };
            var locals = new Dictionary<string, Variable>();

            Assert.IsTrue(ParsedExpression.Parse("a[2]").Evaluate(globals, locals).IsEmpty);
            Assert.IsTrue(globals["a"].IsArray);
            Assert.IsTrue(ParsedExpression.Parse("a[2+2]").Evaluate(globals, locals).IsEmpty);
            Assert.IsTrue(ParsedExpression.Parse("a[b]").Evaluate(globals, locals).IsEmpty);
            Assert.IsTrue(ParsedExpression.Parse("a[2][2]").Evaluate(globals, locals).IsEmpty);
            Assert.IsTrue(globals["a"].NthItem(2).IsArray);
        }

        [TestMethod]
        public void TestEquals()
        {
            var globals = new Dictionary<string, Variable>
            {
                {"a", Variable.Const(2)},
                {"b", Variable.Const(3)},
                {"c", Variable.Const(4)}
            };
            var locals = new Dictionary<string, Variable>();

            Assert.AreEqual(ParsedExpression.Parse("a = b").Evaluate(globals, locals).AsDouble, 3);
            Assert.AreEqual(ParsedExpression.Parse("a = b = c").Evaluate(globals, locals).AsDouble, 4);

            Assert.AreEqual(ParsedExpression.Parse("a[1] = 1").Evaluate(globals, locals).AsDouble, 1);
            Assert.AreEqual(ParsedExpression.Parse("a[2] = 2").Evaluate(globals, locals).AsDouble, 2);
            Assert.AreEqual(ParsedExpression.Parse("a[3] = a[2]").Evaluate(globals, locals).AsDouble, 2);
            Assert.IsTrue(globals["a"].IsArray);
            Assert.AreEqual(globals["a"].NthItem(1).AsDouble, 1);
            Assert.AreEqual(globals["a"].NthItem(2).AsDouble, 2);
            Assert.AreEqual(globals["a"].NthItem(3).AsDouble, 2);
        }

        [TestMethod]
        public void TestAutoCreateGlobals()
        {
            var globals = new Dictionary<string, Variable>
            {
                {"a", Variable.Const(2)},
                {"b", Variable.Const(3)}
            };
            var locals = new Dictionary<string, Variable>();

            Assert.AreEqual(ParsedExpression.Parse("c = b+1").Evaluate(globals, locals).AsDouble, 4);
            Assert.IsTrue(globals.ContainsKey("c"));
            Assert.IsTrue(globals["c"].IsDouble);
            Assert.AreEqual(globals["c"].AsDouble, 4);
        }
    }
}