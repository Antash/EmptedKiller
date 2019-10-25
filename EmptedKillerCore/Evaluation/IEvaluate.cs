namespace EmptedKillerCore.Evaluation
{
    public interface IEvaluate
    {
        const float MaxEvaluation = 500;

        float Evaluate(IPosition position);
    }
}
