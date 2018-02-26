﻿using System;

namespace Diff.Reductions.Compilation
{
    internal class CompilationErrorEventArgs : EventArgs
    {
        public CompilationErrorEventArgs(int line, string errorText)
        {
            Line = line;
            ErrorText = errorText;
        }

        public int Line { get; }
        public string ErrorText { get; }
    }
}