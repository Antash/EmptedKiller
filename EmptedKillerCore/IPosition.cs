using System.Collections.Generic;

namespace EmptedKillerCore
{
    public interface IPosition
    {
        bool WhiteToMove { get; }

        List<Move> Moves { get; }

        Castling WhiteCastling { get; }

        Castling BlackCastling { get; }

        bool EnPassant { get; }

        int EnPassantFile { get; }

        int HalfMoveClock { get; }

        int TotalMoves { get; }

        int EnPassantRank { get; }

        Piece GetPiece(int rank, int file);

        IEnumerable<Move> GetValidMoves();

        IEnumerable<object> GetValidMoves(int rank, int file);

        IPosition MakeMove(Move move);
    }
}