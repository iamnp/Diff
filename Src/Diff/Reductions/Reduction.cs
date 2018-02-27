using System.Reflection;
using Diff.Expressions;

namespace Diff.Reductions
{
    internal class Reduction
    {
        private readonly MethodInfo _method;

        public Reduction(MethodInfo method, string name, string code)
        {
            _method = method;
            Code = code;
            Name = name;
        }

        public string Code { get; }
        public string Name { get; }

        public double Perform(GlobalScope.SearchInterval interval, AssignmentStatement statement)
        {
            var v = new double[interval.End - interval.Start + 1];
            for (var i = interval.Start; i <= interval.End; ++i)
            {
                v[i - interval.Start] = statement.Assignee.NthItem(i).AsDouble;
            }

            return (double) _method.Invoke(null, new object[] {v});
        }

        public override string ToString()
        {
            return Name;
        }
    }
}