using System.Reflection;

namespace Diff.Reductions
{
    internal class Reduction
    {
        private MethodInfo _method;

        public Reduction(MethodInfo method, string name, string code)
        {
            _method = method;
            Code = code;
            Name = name;
        }

        public string Code { get; }
        public string Name { get; }

        public double Perform()
        {
            // TODO call method and return computed reduction
            return 1;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}