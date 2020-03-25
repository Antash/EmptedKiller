using System.Linq;

namespace EmptedKillerCore.Evaluation
{
    public class PieceActivityEvaluation : IEvaluate
    {
        public int Evaluate(IPosition position)
        {
            int result = 0;

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

        private int GetPieceActivity(IPosition position, int rank, int file)
        {
            return position.GetPiece(rank, file).NoColor() switch
            {
                Piece.Knight => position.GetValidMoves(rank, file).Count() * 10,
                Piece.Bishop => position.GetValidMoves(rank, file).Count() * 10,
                Piece.Rook => position.GetValidMoves(rank, file).Count() * 5,
                Piece.Queen => position.GetValidMoves(rank, file).Count() * 3,
                _ => 0,
            };
        }
    }
}
