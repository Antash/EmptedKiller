using System.Diagnostics;

namespace EmptedKillerCore
{
    public class Move
    {
        public Move(int fromRank, int fromFile, int toRank, int toFile)
        {
            FromRank = fromRank;
            FromFile = fromFile;
            ToRank = toRank;
            ToFile = toFile;
        }

        public Move(int fromRank, int fromFile, int toRank, int toFile, Piece piece)
            : this(fromRank, fromFile, toRank, toFile)
        {
            PromotionPiece = piece;
        }

        public int FromRank { get; }

        public int FromFile { get; }

        public int ToRank { get; }

        public int ToFile { get; }

        public Piece? PromotionPiece { get; }

        public override string ToString()
        {
            return NotationHelper.GetMoveCode(this);
        }
    }
}