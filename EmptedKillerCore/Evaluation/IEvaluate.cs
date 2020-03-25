namespace EmptedKillerCore.Evaluation
{
    public interface IEvaluate
    {
        static readonly int MaxEvaluation = 50000;

        int Evaluate(IPosition position);
    }
}
