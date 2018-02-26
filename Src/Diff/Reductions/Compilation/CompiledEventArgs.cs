using System;

namespace Diff.Reductions.Compilation
{
    internal class CompiledEventArgs : EventArgs
    {
        public CompiledEventArgs(Reduction reduction)
        {
            Reduction = reduction;
        }

        public Reduction Reduction { get; }
    }
}