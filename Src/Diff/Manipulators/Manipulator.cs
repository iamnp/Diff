using Diff.Expressions;

namespace Diff.Manipulators
{
    internal class Manipulator
    {
        public readonly InitialValueManipulator InitialValueManipulator;
        public readonly ReductionsManipulator ReductionsManipulator;
        public readonly SearcnIntervalManipulator SearcnIntervalManipulator;

        public Manipulator(MainGraphicOutput mainGraphic, GlobalScope gs)
        {
            ReductionsManipulator = new ReductionsManipulator(mainGraphic, gs);
            InitialValueManipulator = new InitialValueManipulator(mainGraphic, gs);
            SearcnIntervalManipulator = new SearcnIntervalManipulator(mainGraphic, gs);
        }
    }
}