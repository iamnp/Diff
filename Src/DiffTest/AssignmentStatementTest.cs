using System.Collections.Generic;
using Diff.Expressions;
using Diff.Expressions.LowLevel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiffTest
{
    [TestClass]
    public class AssignmentStatementTest
    {
        [TestMethod]
        public void TestSimpleEvaluate()
        {
            var s = new AssignmentStatement();
            s.SetExprString("a = 1.2 + 1");

            var globals = new Dictionary<string, Variable>();
            s.Evaluate(globals);
            Assert.AreEqual(s.Assignee.AsDouble, 2.2);
            Assert.AreEqual(s.Assignee.Name, "a");
            Assert.AreEqual(globals["a"].AsDouble, 2.2);
        }

        [TestMethod]
        public void TestParent()
        {
            var s = new AssignmentStatement();
            s.SetExprString("a[1] = 1");

            var globals = new Dictionary<string, Variable>();
            s.Evaluate(globals);
            Assert.AreEqual(s.Assignee.AsDouble, 1);
            Assert.AreEqual(s.Assignee.Parent, globals["a"]);
            Assert.AreEqual(globals["a"].IsArray, true);
            Assert.AreEqual(globals["a"].NthItem(1).Parent, globals["a"]);
        }
    }
}