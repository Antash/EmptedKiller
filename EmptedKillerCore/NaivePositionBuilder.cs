using System.Collections.Generic;

namespace EmptedKillerCore
{
    public class NaivePositionBuilder : IPositionBuilder
    {
        public bool WhiteToMove { get; private set; }

        public List<Move> Moves { get; } = new List<Move>();

        public Castling WhiteCastling { get; private set; }

        public Castling BlackCastling { get; private set; }

        public bool EnPassant { get; private set; }

        public int EnPassantFile { get; private set; }

        public int HalfMoveClock { get; private set; }

        public int TotalMoves { get; private set; }

        public int EnPassantRank { get; private set; }

        public sbyte[,] Board { get; } = new sbyte[8, 8];

        public IPosition Build()
        {
            return new NaivePosition(this);
        }

        public void PutPiece(int rank, int file, Piece piece, bool isWhite)
        {
            Board[rank, file] = (sbyte)(isWhite ? (int)piece : -(int)piece);
        }

        public void SetCastling(bool forWhite, Castling flags)
        {
            if (forWhite)
            {
                WhiteCastling = flags;
            }
            else
            {
                BlackCastling = flags;
            }
        }

        public void SetEnPassant(int rank, int file)
        {
            EnPassant = true;
            EnPassantRank = rank;
            EnPassantFile = file;
        }

        public void SetFullMoves(int value)
        {
            TotalMoves = value;
        }

        public void SetHalfMoves(int value)
        {
            HalfMoveClock = value;
        }

        public void SetWhiteToMove(bool value)
        {
            WhiteToMove = value;
        }
    }
}
