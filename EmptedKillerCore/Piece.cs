using System;

namespace EmptedKillerCore
{
    [Flags]
    public enum Piece : sbyte
    {
        None = 0,

        Pawn = 1,
        Knight = 2,
        Bishop = 4,
        Rook = 8,
        Queen = 16,
        King = 32,

        Black = 64
    }
}
