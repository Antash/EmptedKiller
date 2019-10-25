using System.Collections.Generic;
using System.Linq;

namespace EmptedKillerCore.Evaluation
{
    public class EvaluationCore : IEvaluate
    {
        private readonly IEnumerable<IEvaluate> _evaluationModules;

        public EvaluationCore(params IEvaluate[] evaluationModules)
        {
            _evaluationModules = evaluationModules;
        }

        public float Evaluate(IPosition position)
        {
            // Check is one side already win
            // (king capture, max abs evaluation)
            bool whiteHasKing = false;
            bool blackHasKing = false;

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var piece = position.GetPiece(rank, file);
                    if (piece == Piece.King)
                    {
                        whiteHasKing = true;
                    }
                    else if (piece == (Piece.King | Piece.Black))
                    {
                        blackHasKing = true;
                    }
                }
            }

            if (!whiteHasKing)
            {
                return -IEvaluate.MaxEvaluation;
            }
            if (!blackHasKing)
            {
                return IEvaluate.MaxEvaluation;
            }

            var eval = _evaluationModules.Sum(module => module.Evaluate(position));
            return eval;
        }
    }
}
