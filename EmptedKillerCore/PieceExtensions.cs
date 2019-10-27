namespace EmptedKillerCore
{
    public static class PieceExtensions
    {
        public static Piece NoColor(this Piece piece)
        {
            return piece & ~Piece.Black;
        }
    }
}
