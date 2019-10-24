namespace EmptedKillerCore
{
    public interface IPositionBuilder
    {
        void PutPiece(int rank, int file, Piece piece, bool isWhite);

        void SetWhiteToMove(bool value);

        void SetHalfMoves(int value);

        void SetFullMoves(int value);

        void SetEnPassant(int rank, int file);

        void SetCastling(bool forWhite, Castling flags);

        IPosition Build();
    }
}