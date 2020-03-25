using EmptedKillerCore.Engine;
using EmptedKillerCore.Evaluation;
using NUnit.Framework;

namespace EmptedKillerCore.Tests
{
    public class ForcedMateTests
    {
        static FenSerializer<NaivePositionBuilder> _serializer = new FenSerializer<NaivePositionBuilder>();
        static IEvaluate _evaluation = new EvaluationCore(
            new PieceValueEvaluation(),
            new PieceActivityEvaluation()
            );

        [Test]
        [TestCase("r1b1kbnr/pppn1ppp/3p2q1/4p3/1P5P/P4P2/1BPPP1P1/RN1QKBNR b KQkq -", "f6f3")]
        public void MateInOne(string fen, string solutionMoveCode)
        {
            var position = _serializer.Read(fen);
            var eng = new EngineRecursive(_evaluation);
        }
    }
}
