using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace Diff.Reductions
{
    internal class ReductionCompiler
    {
        public const string ClassName = "Reduction";
        public const string MethodName = "perform";
        private const int LinesOffset = 4;

        private const string ClassTemplate = @"
static class " + ClassName + @" {{
    {0}
}}
";

        private const string MethodTemplate = @"
public static double " + MethodName + @"(double[] selection) {{
    {0}  
}}
";

        private readonly CSharpCodeProvider _compiler = new CSharpCodeProvider();

        private readonly CompilerParameters _compilerParameters = new CompilerParameters
        {
            GenerateExecutable = false,
            GenerateInMemory = true,
            IncludeDebugInformation = false
        };

        public string LastErrorText { get; private set; }
        public int LastErrorLine { get; private set; }

        public Reduction Compile(string name, string methodCode)
        {
            LastErrorText = null;
            LastErrorLine = -1;

            var results = _compiler.CompileAssemblyFromSource(_compilerParameters,
                string.Format(ClassTemplate, string.Format(MethodTemplate, methodCode)));
            if (results.Errors.Count == 0)
            {
                return new Reduction(
                    results.CompiledAssembly.GetType(ClassName).GetMethod(MethodName, BindingFlags.Static), name,
                    methodCode);
            }

            LastErrorLine = results.Errors[0].Line - LinesOffset;
            LastErrorText = results.Errors[0].ErrorText;

            return null;
        }
    }
}