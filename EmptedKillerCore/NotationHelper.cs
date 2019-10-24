﻿using System;

namespace EmptedKillerCore
{
    public static class NotationHelper
    {
        public static char GetPieceChar(Piece piece)
        {
            return piece switch
            {
                Piece.Pawn => 'p',
                Piece.Knight => 'n',
                Piece.Bishop => 'b',
                Piece.Rook => 'r',
                Piece.Queen => 'q',
                Piece.King => 'k',

                _ => throw new ArgumentException("Unsupported piece", nameof(piece)),
            };
        }

        public static Piece GetPiece(char code)
        {
            return (char.ToLower(code)) switch
            {
                'p' => Piece.Pawn,
                'n' => Piece.Knight,
                'b' => Piece.Bishop,
                'r' => Piece.Rook,
                'q' => Piece.Queen,
                'k' => Piece.King,
                _ => throw new ArgumentException("Unsupported piece char", nameof(code)),
            };
        }

        public static string GetMoveCode(Move move)
        {
            return $"{(char)('a' + move.FromFile)}{8 - move.FromRank}{(char)('a' + move.ToFile)}{8 - move.ToRank}{(move.PromotionPiece.HasValue ? GetPieceChar(move.PromotionPiece.Value).ToString() : string.Empty)}";
        }

        public static Move ParseMoveCode(string moveCode)
        {
            var startFile = moveCode[0] - 'a';
            var startRank = 7 - (moveCode[1] - '1');
            var endFile = moveCode[2] - 'a';
            var endRank = 7 - (moveCode[3] - '1');
            if (moveCode.Length == 5)
            {
                return new Move(startRank, startFile, endRank, endFile, GetPiece(moveCode[4]));
            }
            else
            {
                return new Move(startRank, startFile, endRank, endFile);
            }
        }
    }
}
