namespace EmptedKillerCore
{
    public class EvaluationCore : IEvaluate
    {
        public float Evaluate(IPosition position)
        {
            float result = 0;
            bool whiteHasKing = false;
            bool blackHasKing = false;

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var piece = position.GetPiece(rank, file);
                    var pieceValue = GetPieceValue(position, rank, file);
                    result = position.IsWhitePiece(rank, file)
                        ? result + pieceValue : result - pieceValue;
                    if (piece == Piece.King)
                    {
                        if (position.IsWhitePiece(rank, file))
                        {
                            whiteHasKing = true;
                        }
                        else
                        {
                            blackHasKing = true;
                        }
                    }
                }
            }

            result = !whiteHasKing ? -IEvaluate.MaxEvaluation : result;
            result = !blackHasKing ? IEvaluate.MaxEvaluation : result;

            return result;
        }

        private float GetPieceValue(IPosition position, int rank, int line)
        {
            return position.GetPiece(rank, line) switch
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
