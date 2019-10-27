namespace EmptedKillerCore.Evaluation
{
    public interface IEvaluate
    {
        static readonly float MaxEvaluation = 500;

        float Evaluate(IPosition position);
    }
}
