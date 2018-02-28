using System.CodeDom.Compiler;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace Diff.Reductions.Compilation
{
    public class ReductionCompiler
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

        private readonly object _locker = new object();
        private readonly SynchronizationContext _synchronizationContext;
        private Task _compilingTask;
        private string _pendingCode;
        private string _pendingName;

        public ReductionCompiler(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        public event CompiledHandler Compiled;
        public event CompilationErrorHandler CompilationError;

        public void Compile(string name, string methodCode)
        {
            lock (_locker)
            {
                if (_compilingTask == null)
                {
                    _compilingTask = Task.Factory.StartNew(() => CompileInternal(name, methodCode));
                }
                else
                {
                    _pendingCode = methodCode;
                    _pendingName = name;
                }
            }
        }

        private void CompileInternal(string name, string methodCode)
        {
            var compiledSuccessfully = false;

            var results = _compiler.CompileAssemblyFromSource(_compilerParameters,
                string.Format(ClassTemplate, string.Format(MethodTemplate, methodCode)));
            if (results.Errors.Count == 0)
            {
                compiledSuccessfully = true;
                var method = results.CompiledAssembly.GetType(ClassName)
                    .GetMethod(MethodName, BindingFlags.Static | BindingFlags.Public);
                _synchronizationContext.Send(
                    o => Compiled?.Invoke(this, new CompiledEventArgs(new Reduction(
                        method, name, methodCode))), null);
            }

            if (!compiledSuccessfully)
            {
                _synchronizationContext.Send(
                    o =>
                        CompilationError?.Invoke(this,
                            new CompilationErrorEventArgs(results.Errors[0].Line - LinesOffset,
                                results.Errors[0].ErrorText)), null);
            }

            lock (_locker)
            {
                _compilingTask = null;
                if (_pendingCode != null)
                {
                    var tmpCode = string.Copy(_pendingCode);
                    var tmpName = string.Copy(_pendingName);
                    _compilingTask = Task.Factory.StartNew(() => CompileInternal(tmpName, tmpCode));
                    _pendingCode = null;
                    _pendingName = null;
                }
            }
        }
    }
}