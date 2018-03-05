using Diff.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiffTest
{
    [TestClass]
    public class DerivativeTest
    {
        [TestMethod]
        public void TestSimpleDerivative()
        {
            var gs = new GlobalScope();

            gs.AssignmentStatements.Add(new AssignmentStatement());
            gs.AssignmentStatements[0].SetExprString("a = 1");
            gs.Evaluate();
            gs.AssignmentStatements[0].SetExprString("a[n+1] = a[n] + 0.5454");
            gs.AssignmentStatements.Add(new AssignmentStatement());
            gs.AssignmentStatements[1].SetExprString("b[n] = der(a[n])");
            gs.Evaluate();

            Assert.AreEqual(gs.Globals["b"].NthItem(0).AsDouble, 0.5454, 0.000001);
            Assert.AreEqual(gs.Globals["b"].NthItem(1).AsDouble, 0.5454, 0.000001);
            Assert.AreEqual(gs.Globals["b"].NthItem(2).AsDouble, 0.5454, 0.000001);
        }
    }
}