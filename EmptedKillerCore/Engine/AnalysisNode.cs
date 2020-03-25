namespace EmptedKillerCore.Engine
{
    public struct AnalysisNode
    {
        public AnalysisNode(IPosition position, int evaluation, int depth)
        {
            Position = position;
            Evaluation = evaluation;
            Depth = depth;
        }

        public IPosition Position { get; }

        public int Evaluation { get; }

        public int Depth { get; }
    }
}
