using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmptedKillerCore.Evaluation
{
    public class PieceActivityEvaluation : IEvaluate
    {
        public float Evaluate(IPosition position)
        {
            float result = 0;

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var pieceActivity = GetPieceActivity(position, rank, file);
                    result = (position.GetPiece(rank, file) & Piece.Black) == 0
                        ? result + pieceActivity : result - pieceActivity;
                }
            }

            return result;
        }

        private float GetPieceActivity(IPosition position, int rank, int file)
        {
            return position.GetPiece(rank, file) switch
            {
                Piece.Knight => position.GetValidMoves(rank, file).Count() * 0.1f,
                Piece.Bishop => position.GetValidMoves(rank, file).Count() * 0.1f,
                Piece.Rook => position.GetValidMoves(rank, file).Count() * 0.07f,
                Piece.Queen => position.GetValidMoves(rank, file).Count() * 0.04f,
                _ => 0,
            };
        }
    }
}
