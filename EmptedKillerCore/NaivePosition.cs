using System;
using System.Collections.Generic;
using System.Linq;

namespace EmptedKillerCore
{
    public class NaivePosition : IPosition
    {
        // White - positive piece value
        // Black - negative piece value
        // 1 - Pawn
        // 2 - Knight
        // 3 - Bishop
        // 4 - Rook
        // 5 - Queen
        // 6 - King
        private static readonly (int, int)[] knightMoveOffsets = new[] { (1, 2), (2, 1), (-1, 2), (1, -2), (-2, 1), (2, -1), (-1, -2), (-2, -1) };
        private static readonly (int, int)[] kingMoveOffsets = new[] { (1, 1), (0, 1), (1, 0), (1, -1), (-1, 1), (0, -1), (-1, 0), (-1, -1) };

        private readonly sbyte[,] _board = new sbyte[8, 8];

        public NaivePosition()
        {
            Array.Clear(_board, 0, _board.Length);
            for (int i = 0; i < 8; i++)
            {
                _board[1, i] = -1;
                _board[6, i] = 1;
            }
            _board[0, 0] = _board[0, 7] = -4;
            _board[7, 0] = _board[7, 7] = 4;
            _board[0, 1] = _board[0, 6] = -2;
            _board[7, 1] = _board[7, 6] = 2;
            _board[0, 2] = _board[0, 5] = -3;
            _board[7, 2] = _board[7, 5] = 3;
            _board[0, 3] = -5;
            _board[0, 4] = -6;
            _board[7, 3] = 5;
            _board[7, 4] = 6;

            WhiteCastling = BlackCastling = Castling.KingSide | Castling.QueenSide;
            WhiteToMove = true;
            TotalMoves = HalfMoveClock = 0;
        }

        public NaivePosition(NaivePositionBuilder builder)
        {
            Array.Copy(builder.Board, 0, _board, 0, builder.Board.Length);
            WhiteToMove = builder.WhiteToMove;
            EnPassant = builder.EnPassant;
            EnPassantFile = builder.EnPassantFile;
            EnPassantRank = builder.EnPassantRank;
            WhiteCastling = builder.WhiteCastling;
            BlackCastling = builder.BlackCastling;
            TotalMoves = builder.TotalMoves;
            HalfMoveClock = builder.HalfMoveClock;
            Moves = new List<Move>(builder.Moves);
        }

        public NaivePosition(NaivePosition position)
        {
            Array.Copy(position._board, 0, _board, 0, position._board.Length);
            WhiteToMove = position.WhiteToMove;
            EnPassant = position.EnPassant;
            EnPassantFile = position.EnPassantFile;
            EnPassantRank = position.EnPassantRank;
            WhiteCastling = position.WhiteCastling;
            BlackCastling = position.BlackCastling;
            TotalMoves = position.TotalMoves;
            HalfMoveClock = position.HalfMoveClock;
            Moves = new List<Move>(position.Moves);
        }

        public List<Move> Moves { get; } = new List<Move>();

        public bool WhiteToMove { get; private set; }

        public Castling WhiteCastling { get; private set; }

        public Castling BlackCastling { get; private set; }

        public bool EnPassant { get; private set; }

        public int EnPassantRank { get; private set; }

        public int EnPassantFile { get; private set; }

        public int HalfMoveClock { get; private set; }

        public int TotalMoves { get; private set; }

        private IList<Move> GetValidMovesInternal(bool opponentMoves = false, bool potentialCapture = false, bool onlyCapture = false)
        {
            var whiteMoves = opponentMoves ? !WhiteToMove : WhiteToMove;
            List<Move> moves = new List<Move>();

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var accessibleSqares = Enumerable.Empty<(int, int)>();
                    var piece = _board[rank, file];
                    if (piece > 0 && whiteMoves || piece < 0 && !whiteMoves)
                    {
                        switch (Math.Abs(piece))
                        {
                            case 1:
                                accessibleSqares = GetPawnAccessibleSquares(rank, file, -Math.Sign(piece), potentialCapture);
                                break;
                            case 2:
                                accessibleSqares = GetKnightAccessibleSqares(rank, file, -Math.Sign(piece));
                                break;
                            case 3:
                                accessibleSqares = GetBishopAccessibleSqares(rank, file, -Math.Sign(piece));
                                break;
                            case 4:
                                accessibleSqares = GetRookAccessibleSqares(rank, file, -Math.Sign(piece));
                                break;
                            case 5:
                                accessibleSqares = GetBishopAccessibleSqares(rank, file, -Math.Sign(piece))
                                    .Concat(GetRookAccessibleSqares(rank, file, -Math.Sign(piece)));
                                break;
                            case 6:
                                accessibleSqares = GetKingAccessibleSqares(rank, file, -Math.Sign(piece), potentialCapture);
                                break;
                        }
                    }

                    foreach(var square in accessibleSqares)
                    {
                        var newRank = square.Item1;
                        var newFile = square.Item2;
                        if (onlyCapture && _board[newRank, newFile] == 0)
                        {
                            continue;
                        }
                        if (Math.Abs(piece) == 1 && (newRank == 0 || newRank == 7))
                        {
                            // Pawn promotion moves
                            for (sbyte newPiece = 5; newPiece > 1; newPiece--)
                            {
                                moves.Add(new Move(rank, file, newRank, newFile, (Piece)newPiece));
                            }
                        }
                        else
                        {
                            moves.Add(new Move(rank, file, newRank, newFile));
                        }
                    }
                }
            }

            return moves;
        }

        private IEnumerable<(int, int)> GetBishopAccessibleSqares(int rank, int file, int capturePieceSign)
        {
            var allSqares = GetSqaresUntillObstacle(rank, file, 1, 1, capturePieceSign)
                .Concat(GetSqaresUntillObstacle(rank, file, 1, -1, capturePieceSign))
                .Concat(GetSqaresUntillObstacle(rank, file, -1, 1, capturePieceSign))
                .Concat(GetSqaresUntillObstacle(rank, file, -1, -1, capturePieceSign));
            foreach (var sqare in allSqares)
            {
                yield return sqare;
            }
        }

        private IEnumerable<(int, int)> GetRookAccessibleSqares(int rank, int file, int capturePieceSign)
        {
            var allSqares = GetSqaresUntillObstacle(rank, file, 1, 0, capturePieceSign)
                .Concat(GetSqaresUntillObstacle(rank, file, 0, 1, capturePieceSign))
                .Concat(GetSqaresUntillObstacle(rank, file, -1, 0, capturePieceSign))
                .Concat(GetSqaresUntillObstacle(rank, file, 0, -1, capturePieceSign));
            foreach (var sqare in allSqares)
            {
                yield return sqare;
            }
        }

        private IEnumerable<(int, int)> GetSqaresUntillObstacle(int rank, int file, int rankOffset, int fileOffset, int capturePieceSign)
        {
            while(true)
            {
                rank += rankOffset;
                file += fileOffset;
                if (!IsOnBoard(rank, file))
                {
                    yield break;
                }
                if (_board[rank, file] * capturePieceSign >= 0)
                {
                    yield return (rank, file);
                }
                if (_board[rank, file] != 0)
                {
                    yield break;
                }
            }
        }

        private IEnumerable<(int, int)> GetKingAccessibleSqares(int rank, int file, int capturePieceSign, bool potentialCapture)
        {
            foreach (var offset in kingMoveOffsets)
            {
                if (IsOnBoard(rank + offset.Item1, file + offset.Item2) && 
                    _board[rank + offset.Item1, file + offset.Item2] * capturePieceSign >= 0)
                {
                    yield return (rank + offset.Item1, file + offset.Item2);
                }
            }

            if (potentialCapture)
            {
                yield break;
            }
            // Castling
            var castlingFlags = WhiteToMove ? WhiteCastling : BlackCastling;
            var rankToCheck = WhiteToMove ? 7 : 0;
            if (castlingFlags != Castling.None)
            {
                if ((castlingFlags & Castling.KingSide) != 0 && 
                    _board[rankToCheck, 5] == 0 && 
                    _board[rankToCheck, 6] == 0 &&
                    !IsUnderThreat(rankToCheck, 5))
                {
                    yield return (rankToCheck, 6);
                }
                if ((castlingFlags & Castling.QueenSide) != 0 &&
                    _board[rankToCheck, 1] == 0 &&
                    _board[rankToCheck, 2] == 0 &&
                    _board[rankToCheck, 3] == 0 &&
                    !IsUnderThreat(rankToCheck, 3))
                {
                    yield return (rankToCheck, 2);
                }
            }
        }

        private bool IsUnderThreat(int rank, int file)
        {
            return GetValidMovesInternal(!WhiteToMove, true)
                .Any(move => move.ToRank == rank && move.ToFile == file);
        }

        private IEnumerable<(int, int)> GetKnightAccessibleSqares(int rank, int file, int capturePieceSign)
        {
            foreach (var offset in knightMoveOffsets)
            {
                if (IsOnBoard(rank + offset.Item1, file + offset.Item2) && _board[rank + offset.Item1, file + offset.Item2] * capturePieceSign >= 0)
                {
                    yield return (rank + offset.Item1, file + offset.Item2);
                }
            }
        }

        private IEnumerable<(int, int)> GetPawnAccessibleSquares(int rank, int file, int offset, bool captureOnly)
        {
            if (!captureOnly)
            {
                // White or black start pawn rank
                int startPawnRank = offset < 0 ? 6 : 1;
                if (rank == startPawnRank && _board[startPawnRank + offset, file] == 0 && _board[startPawnRank + 2 * offset, file] == 0)
                {
                    // Long move available
                    yield return (startPawnRank + 2 * offset, file);
                }
                if (_board[rank + offset, file] == 0)
                {
                    // Short move
                    yield return (rank + offset, file);
                }
            }
            // Captures
            if (file - 1 >= 0 && _board[rank + offset, file - 1] * offset > 0)
            {
                yield return (rank + offset, file - 1);
            }
            if (file + 1 < 8 && _board[rank + offset, file + 1] * offset > 0)
            {
                yield return (rank + offset, file + 1);
            }
            if (EnPassant && 
                (EnPassantFile == file + 1 && EnPassantRank == rank + offset ||
                EnPassantFile == file - 1 && EnPassantRank == rank + offset))
            {
                yield return (EnPassantRank, EnPassantFile);
            }
        }

        private static bool IsOnBoard(int rank, int file)
        {
            return rank < 8 && rank >= 0 && file < 8 && file >= 0;
        }

        public Piece GetPiece(int rank, int file)
        {
            return (Piece)Math.Abs(_board[rank, file]);
        }

        public bool IsWhitePiece(int rank, int file)
        {
            return _board[rank, file] > 0;
        }

        public IEnumerable<Move> GetValidMoves()
        {
            return GetValidMovesInternal();
        }

        public IPosition MakeMove(Move move)
        {
            var position = new NaivePosition(this);
            position.Moves.Add(move);
            position.HalfMoveClock++;

            bool enPassantJustSet = false;

            var piece = position._board[move.FromRank, move.FromFile];
            if (move.PromotionPiece.HasValue)
            {
                // Pawn promotion
                piece = (sbyte)(Math.Sign(piece) * (sbyte)move.PromotionPiece.Value);
            }
            // Capture: clear halfmove counter
            if (position._board[move.ToRank, move.ToFile] != 0)
            {
                position.HalfMoveClock = 0;
            }
            position._board[move.FromRank, move.FromFile] = 0;
            position._board[move.ToRank, move.ToFile] = piece;

            switch ((Piece)Math.Abs(piece))
            {
                case Piece.Pawn:
                    // En-passant capture
                    if (position.EnPassant && position.EnPassantRank == move.ToRank && position.EnPassantFile == move.ToFile)
                    {
                        position._board[move.ToRank == 5 ? 4 : 3, move.ToFile] = 0;
                    }
                    // Long move
                    else if (Math.Abs(move.ToRank - move.FromRank) > 1)
                    {
                        enPassantJustSet = true;
                        position.EnPassant = true;
                        position.EnPassantFile = move.FromFile;
                        position.EnPassantRank = move.FromRank == 1 ? 2 : 5;
                    }
                    // Clear half move counter on any pawn move
                    position.HalfMoveClock = 0;
                    break;
                case Piece.King:
                    // Castling
                    if (Math.Abs(move.ToFile - move.FromFile) > 1)
                    {
                        var rook = position._board[move.ToRank, move.ToFile == 2 ? 0 : 7];
                        position._board[move.ToRank, move.ToFile == 2 ? 0 : 7] = 0;
                        position._board[move.ToRank, move.ToFile == 2 ? 3 : 5] = rook;
                    }
                    // Unset castling when the king moves
                    if (position.WhiteToMove)
                    {
                        position.WhiteCastling = Castling.None;
                    }
                    else
                    {
                        position.BlackCastling = Castling.None;
                    }
                    break;
                case Piece.Rook:
                    // Unset castling flag when rook moves
                    if (position.WhiteToMove)
                    {
                        position.WhiteCastling &= move.FromFile == 7 ? ~Castling.KingSide : ~Castling.QueenSide;
                    }
                    else
                    {
                        position.BlackCastling &= move.FromFile == 7 ? ~Castling.KingSide : ~Castling.QueenSide;
                    }
                    break;
            }

            if (!position.WhiteToMove)
            {
                position.TotalMoves++;
            }

            position.WhiteToMove = !position.WhiteToMove;
            if (!enPassantJustSet)
            {
                position.EnPassant = false;
            }

            return position;
        }
    }
}
