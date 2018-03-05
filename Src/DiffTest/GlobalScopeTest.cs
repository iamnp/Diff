using Diff.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiffTest
{
    [TestClass]
    public class GlobalScopeTest
    {
        [TestMethod]
        public void TestIterationsSingle()
        {
            var gs = new GlobalScope();

            gs.AssignmentStatements.Add(new AssignmentStatement());
            gs.AssignmentStatements[0].SetExprString("a = 1");
            gs.Evaluate();
            Assert.AreEqual(gs.Globals["a"].AsDouble, 1.0);

            gs.AssignmentStatements[0].SetExprString("a[n+1] = a[n] + 1");
            gs.Evaluate();
            Assert.AreEqual(gs.Globals["a"].IsArray, true);
            Assert.AreEqual(gs.Globals["a"].AsDouble, 1.0);
            Assert.AreEqual(gs.Globals["a"].NthItem(0).AsDouble, 1.0);
            Assert.AreEqual(gs.Globals["a"].NthItem(1).AsDouble, 2.0);
            Assert.AreEqual(gs.Globals["a"].NthItem(2).AsDouble, 3.0);
        }

        [TestMethod]
        public void TestIterationsTwo()
        {
            var gs = new GlobalScope();

            gs.AssignmentStatements.Add(new AssignmentStatement());
            gs.AssignmentStatements[0].SetExprString("a = 1");
            gs.Evaluate();
            Assert.AreEqual(gs.Globals["a"].AsDouble, 1.0);

            gs.AssignmentStatements.Add(new AssignmentStatement());
            gs.AssignmentStatements[1].SetExprString("b = 2");
            gs.Evaluate();
            Assert.AreEqual(gs.Globals["b"].AsDouble, 2.0);

            gs.AssignmentStatements[0].SetExprString("a[n+1] = a[n] - 0.01*b[n]");
            gs.AssignmentStatements[1].SetExprString("b[n+1] = b[n] + 0.01*a[n+1]");
            gs.Evaluate();
            Assert.AreEqual(gs.Globals["a"].IsArray, true);
            Assert.AreEqual(gs.Globals["b"].IsArray, true);
        }
    }
}