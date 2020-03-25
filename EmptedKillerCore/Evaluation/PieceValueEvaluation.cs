namespace EmptedKillerCore.Evaluation
{
    public class PieceValueEvaluation : IEvaluate
    {
        public int Evaluate(IPosition position)
        {
            int result = 0;

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

        private int GetPieceValue(IPosition position, int rank, int file)
        {
            return (position.GetPiece(rank, file).NoColor()) switch
            {
                Piece.Pawn => 100,
                Piece.Knight => 300,
                Piece.Bishop => 300,
                Piece.Rook => 500,
                Piece.Queen => 900,

                _ => 0,
            };
        }
    }
}
