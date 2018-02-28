using System.Threading;
using Diff.Expressions;
using Diff.Reductions;
using Diff.Reductions.Compilation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiffTest
{
    [TestClass]
    public class ReductionsTest
    {
        [TestMethod]
        public void TestMeanReduction()
        {
            var gs = new GlobalScope();

            gs.AssignmentStatements.Add(new AssignmentStatement());
            gs.AssignmentStatements[0].SetExprString("a = 1");
            gs.Evaluate();
            gs.AssignmentStatements[0].SetExprString("a[n+1] = a[n] + 1");
            gs.Evaluate();

            var interval = new GlobalScope.SearchInterval {Start = 0, End = 10};

            Reduction reduction = null;

            var autoResetEvent = new AutoResetEvent(false);
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            var reductionCompiler = new ReductionCompiler(SynchronizationContext.Current);
            reductionCompiler.Compiled += (sender, e) =>
            {
                reduction = e.Reduction;
                autoResetEvent.Set();
            };
            reductionCompiler.CompilationError += (sender, e) => autoResetEvent.Set();
            reductionCompiler.Compile("mean", @"double sum = 0.0;
for (int i = 0; i < selection.Length; ++i) {
    sum += selection[i];
}
return sum/selection.Length;");

            Assert.IsTrue(autoResetEvent.WaitOne());
            Assert.IsNotNull(reduction);
            Assert.AreEqual(6, reduction.Perform(interval, gs.AssignmentStatements[0]));
        }
    }
}