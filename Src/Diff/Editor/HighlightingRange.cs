using System.Drawing;

namespace Diff.Editor
{
    /// <summary>
    ///     Holds info about highlighted text range.
    /// </summary>
    internal class HighlightingRange
    {
        public Color Color;
        public int Length;
        public int Start;
    }
}