namespace EmptedKillerCore.Evaluation
{
    public class PieceValueEvaluation : IEvaluate
    {
        public float Evaluate(IPosition position)
        {
            float result = 0;

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var pieceValue = GetPieceValue(position, rank, file);
                    result = (position.GetPiece(rank, file) & Piece.Black) == 0
                        ? result + pieceValue : result - pieceValue;
                }
            }

            return result;
        }

        private float GetPieceValue(IPosition position, int rank, int file)
        {
            return (position.GetPiece(rank, file).NoColor()) switch
            {
                Piece.Pawn => 1,
                Piece.Knight => 3,
                Piece.Bishop => 3,
                Piece.Rook => 5,
                Piece.Queen => 9,

                _ => 0,
            };
        }
    }
}
