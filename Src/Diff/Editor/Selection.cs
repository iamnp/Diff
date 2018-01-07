﻿namespace Diff.Editor
{
    /// <summary>
    ///     Holds info about text selection.
    /// </summary>
    internal class Selection
    {
        public Position End = new Position();
        public Position Start = new Position();

        public bool IsEmpty => (Start.Column == End.Column) && (Start.Line == End.Line);

        /// <summary>
        ///     Returns a sorted copy of current Selection instance.
        /// </summary>
        public Selection Sorted()
        {
            var sel = new Selection {Start = Start, End = End};
            if ((sel.Start.Line > sel.End.Line) ||
                ((sel.Start.Line == sel.End.Line) && (sel.Start.Column > sel.End.Column)))
            {
                var t = sel.Start;
                sel.Start = sel.End;
                sel.End = t;
            }

            return sel;
        }
    }
}