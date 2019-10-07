using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmptedKiller
{
    public class Position
    {
        // White - positive value
        // Black - negative
        // 1 - Pawn
        // 2 - Knight
        // 3 - Bishop
        // 4 - Rook
        // 5 - Queen
        // 6 - King

        [Flags]
        enum Castling
        {
            None = 0,
            KingSide = 1,
            QueenSide = 2
        }

        // board[rank, file]
        protected readonly sbyte[,] board = new sbyte[8, 8];
        bool enPassant;
        int enPassantRank;
        int enPassantFile;
        Castling whiteCastling;
        Castling blackCastling;
        int totalMoves;
        int halfMoveClock;
        public bool whiteToMove;
        public bool playingWhite;

        static readonly (int, int)[] knightMoveOffsets = new[] { (1, 2), (2, 1), (-1, 2), (1, -2), (-2, 1), (2, -1), (-1, -2), (-2, -1) };
        static readonly (int, int)[] kingMoveOffsets = new[] { (1, 1), (0, 1), (1, 0), (1, -1), (-1, 1), (0, -1), (-1, 0), (-1, -1) };
        static readonly Random rnd = new Random(DateTime.Now.Millisecond);

        float? evaluation = null;

        public Position(bool playingWhite)
        {
            this.playingWhite = playingWhite;

            Array.Clear(board, 0, board.Length);
            for (int i = 0; i < 8; i++)
            {
                board[1, i] = -1;
                board[6, i] = 1;
            }
            board[0, 0] = board[0, 7] = -4;
            board[7, 0] = board[7, 7] = 4;
            board[0, 1] = board[0, 6] = -2;
            board[7, 1] = board[7, 6] = 2;
            board[0, 2] = board[0, 5] = -3;
            board[7, 2] = board[7, 5] = 3;
            board[0, 3] = -5;
            board[0, 4] = -6;
            board[7, 3] = 5;
            board[7, 4] = 6;

            whiteCastling = blackCastling = Castling.KingSide | Castling.QueenSide;
            whiteToMove = true;
            totalMoves = halfMoveClock = 0;
        }

        public Position(Position position)
        {
            playingWhite = position.playingWhite;
            Array.Copy(position.board, 0, board, 0, position.board.Length);
            whiteToMove = position.whiteToMove;
            enPassant = position.enPassant;
            enPassantFile = position.enPassantFile;
            enPassantRank = position.enPassantRank;
            whiteCastling = position.whiteCastling;
            blackCastling = position.blackCastling;
            totalMoves = position.totalMoves;
            halfMoveClock = position.halfMoveClock;
            evaluation = null;
        }

        public float GetEvaluation()
        {
            if (evaluation.HasValue)
            {
                return evaluation.Value;
            }

            float result = 0;
            bool whiteHasKing = false;
            bool blackHasKing = false;

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var piece = board[rank, file];
                    result += Math.Sign(piece) * GetPieceValue(rank, file);
                    if (piece == 6)
                    {
                        whiteHasKing = true;
                    }
                    if (piece == -6)
                    {
                        blackHasKing = true;
                    }
                }
            }

            result = !whiteHasKing ? -500 : result;
            result = !blackHasKing ? 500 : result;

            evaluation = result;

            return evaluation.Value;
        }

        private float GetPieceValue(int rank, int line)
        {
            var piecesToCapture = -Math.Sign(board[rank, line]);
            switch (Math.Abs(board[rank, line]))
            {
                case 1:
                    return 1;
                case 2:
                    return 3 + (GetKnightAccessibleSqares(rank, line, piecesToCapture).Count() - 5) / 3;
                case 3:
                    return 3 + (GetBishopAccessibleSqares(rank, line, piecesToCapture).Count() - 8) / 5;
                case 4:
                    return 5 + (GetRookAccessibleSqares(rank, line, piecesToCapture).Count() - 4) / 10;
                case 5:
                    return 9 + (GetBishopAccessibleSqares(rank, line, piecesToCapture).Count() 
                        + GetRookAccessibleSqares(rank, line, piecesToCapture).Count() - 5) / 15;
                default:
                    return 0;
            }
        }

        internal Line MakeMove(string moveCode, bool branch = false)
        {
            var position = branch ? new Line(this, moveCode) : this;
            position.halfMoveClock++;
            position.evaluation = null;

            var startFile = moveCode[0] - 'a';
            var startRank = 7 - (moveCode[1] - '1');
            var endFile = moveCode[2] - 'a';
            var endRank = 7 - (moveCode[3] - '1');

            bool enPassantJustSet = false;

            var piece = position.board[startRank, startFile];
            if (moveCode.Length == 5)
            {
                // Pawn promotion
                piece = (sbyte)(Math.Sign(piece) * GetPieceId(moveCode[4]));
            }
            // Capture: clear halfmove counter
            if (position.board[endRank, endFile] != 0)
            {
                position.halfMoveClock = 0;
            }
            position.board[startRank, startFile] = 0;
            position.board[endRank, endFile] = piece;

            if (GetPieceChar(piece) == 'p')
            {
                // En passant capture
                if (position.enPassant && position.enPassantRank == endRank && position.enPassantFile == endFile)
                {
                    position.board[endRank == 5 ? 4 : 3, endFile] = 0;
                }
                // Long move
                else if (Math.Abs(endRank - startRank) > 1)
                {
                    enPassantJustSet = true;
                    position.enPassant = true;
                    position.enPassantFile = startFile;
                    position.enPassantRank = startRank == 1 ? 2 : 5;
                }
                // Clear half move counter on any pawn move or capture
                position.halfMoveClock = 0;
            }
            else if (GetPieceChar(piece) == 'k')
            {
                // Castling
                if (Math.Abs(endFile - startFile) > 1)
                {
                    var rook = position.board[endRank, endFile == 2 ? 0 : 7];
                    position.board[endRank, endFile == 2 ? 0 : 7] = 0;
                    position.board[endRank, endFile == 2 ? 3 : 5] = rook;
                }
                // Unset castling when the king moves
                if (position.whiteToMove)
                {
                    position.whiteCastling = Castling.None;
                }
                else
                {
                    position.blackCastling = Castling.None;
                }
            }
            else if ((position.whiteToMove && position.whiteCastling != Castling.None || !position.whiteToMove && position.blackCastling != Castling.None) && GetPieceChar(piece) == 'r')
            {
                // Unset castling flag when rook moves
                if (position.whiteToMove)
                {
                    position.whiteCastling &= startFile == 7 ? ~Castling.KingSide : ~Castling.QueenSide;
                }
                else
                {
                    position.blackCastling &= startFile == 7 ? ~Castling.KingSide : ~Castling.QueenSide;
                }
            }

            if (!position.whiteToMove)
            {
                position.totalMoves++;
            }

            position.whiteToMove = !position.whiteToMove;
            if (enPassantJustSet)
            {
                position.enPassant = false;
            }

            return branch ? (Line) position : null;
        }

        private static sbyte GetPieceId(char code)
        {
            switch(char.ToLower(code))
            {
                case 'p':
                    return 1;
                case 'n':
                    return 2;
                case 'b':
                    return 3;
                case 'r':
                    return 4;
                case 'q':
                    return 5;
                case 'k':
                    return 6;
            }
            throw new ArgumentException();
        }

        protected static char GetPieceChar(sbyte value)
        {
            switch (Math.Abs(value))
            {
                case 1:
                    return 'p';
                case 2:
                    return 'n';
                case 3:
                    return 'b';
                case 4:
                    return 'r';
                case 5:
                    return 'q';
                case 6:
                    return 'k';
                default:
                    return '0';
            }
        }

        internal string GetNextMove()
        {
            var eng = new Engine();
            eng.Analyze(this);

            var ll = eng.lines;
            var maxEval = ll.Max(la => la.GetEvaluation());
            var best = ll.Where(l => Math.Abs(l.GetEvaluation() - maxEval) < float.Epsilon).ToList();
            var theBest = best[rnd.Next(best.Count)];
            return theBest.moves.First();
        }

        public IList<string> GetValidMoves()
        {
            return GetValidMoves(whiteToMove);
        }

        private IList<string> GetValidMoves(bool whitePieces, bool potentialCapture = false, bool onlyCapture = false)
        {
            List<string> moves = new List<string>();

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var accessibleSqares = Enumerable.Empty<(int, int)>();
                    var piece = board[rank, file];
                    if (piece > 0 && whitePieces || piece < 0 && !whitePieces)
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
                        if (onlyCapture && board[newRank, newFile] == 0)
                        {
                            continue;
                        }
                        var moveCode = $"{(char)('a' + file)}{8 - rank}{(char)('a' + newFile)}{8 - newRank}";
                        if (Math.Abs(piece) == 1 && (newRank == 0 || newRank == 7))
                        {
                            // Pawn promotion moves
                            for (sbyte newPiece = 5; newPiece > 1; newPiece--)
                            {
                                moves.Add($"{moveCode}{GetPieceChar(newPiece)}");
                            }
                        }
                        else
                        {
                            moves.Add(moveCode);
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
                if (board[rank, file] * capturePieceSign >= 0)
                {
                    yield return (rank, file);
                }
                if (board[rank, file] != 0)
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
                    board[rank + offset.Item1, file + offset.Item2] * capturePieceSign >= 0)
                {
                    yield return (rank + offset.Item1, file + offset.Item2);
                }
            }

            if (potentialCapture)
            {
                yield break;
            }
            // Castling
            var castlingFlags = whiteToMove ? whiteCastling : blackCastling;
            var rankToCheck = whiteToMove ? 7 : 0;
            if (castlingFlags != Castling.None)
            {
                if ((castlingFlags & Castling.KingSide) != 0 && 
                    board[rankToCheck, 5] == 0 && 
                    board[rankToCheck, 6] == 0 &&
                    !IsUnderThreat(rankToCheck, 5))
                {
                    yield return (rankToCheck, 6);
                }
                if ((castlingFlags & Castling.QueenSide) != 0 &&
                    board[rankToCheck, 1] == 0 &&
                    board[rankToCheck, 2] == 0 &&
                    board[rankToCheck, 3] == 0 &&
                    !IsUnderThreat(rankToCheck, 3))
                {
                    yield return (rankToCheck, 2);
                }
            }
        }

        private bool IsUnderThreat(int rank, int file)
        {
            var targetSqare = $"{(char)('a' + file)}{8 - rank}";
            return GetValidMoves(!whiteToMove, true).Any(move => move.Contains(targetSqare));
        }

        private IEnumerable<(int, int)> GetKnightAccessibleSqares(int rank, int file, int capturePieceSign)
        {
            foreach (var offset in knightMoveOffsets)
            {
                if (IsOnBoard(rank + offset.Item1, file + offset.Item2) && board[rank + offset.Item1, file + offset.Item2] * capturePieceSign >= 0)
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
                if (rank == startPawnRank && board[startPawnRank + offset, file] == 0 && board[startPawnRank + 2 * offset, file] == 0)
                {
                    // Long move available
                    yield return (startPawnRank + 2 * offset, file);
                }
                if (board[rank + offset, file] == 0)
                {
                    // Short move
                    yield return (rank + offset, file);
                }
            }
            // Captures
            if (file - 1 >= 0 && board[rank + offset, file - 1] * offset > 0)
            {
                yield return (rank + offset, file - 1);
            }
            if (file + 1 < 8 && board[rank + offset, file + 1] * offset > 0)
            {
                yield return (rank + offset, file + 1);
            }
            if (enPassant && 
                (enPassantFile == file + 1 && enPassantRank == rank + offset ||
                enPassantFile == file - 1 && enPassantRank == rank + offset))
            {
                yield return (enPassantRank, enPassantFile);
            }
        }

        private static bool IsOnBoard(int rank, int file)
        {
            return rank < 8 && rank >= 0 && file < 8 && file >= 0;
        }

        public override string ToString()
        {
            var res = new StringBuilder();
            for (int rank = 0; rank < 8; rank++)
            {
                int gap = 0;
                for (int file = 0; file < 8; file++)
                {
                    char piece = GetPieceChar(board[rank, file]);
                    piece = board[rank, file] > 0 ? char.ToUpper(piece) : piece;
                    if (piece != '0')
                    {
                        if (gap > 0)
                        {
                            res.Append(gap);
                            gap = 0;
                        }
                        res.Append(piece);
                    }
                    else
                    {
                        gap++;
                    }
                }
                res.Append(gap > 0 ? gap.ToString() : string.Empty);
                if (rank < 7)
                {
                    res.Append("/");
                }
            }

            res.Append($" {(whiteToMove ? 'w' : 'b')}");

            if (whiteCastling != Castling.None || blackCastling != Castling.None)
            {
                res.Append(" ");
                if ((whiteCastling & Castling.KingSide) != 0)
                {
                    res.Append('K');
                }
                if ((whiteCastling & Castling.QueenSide) != 0)
                {
                    res.Append('Q');
                }
                if ((blackCastling & Castling.KingSide) != 0)
                {
                    res.Append('k');
                }
                if ((blackCastling & Castling.QueenSide) != 0)
                {
                    res.Append('q');
                }
            }
            else
            {
                res.Append(" -");
            }

            if (enPassant)
            {
                res.Append($" {(char)('a' + enPassantFile)}{8 - enPassantRank}");
            }
            else
            {
                res.Append(" -");
            }

            res.Append($" {halfMoveClock}");
            res.Append($" {totalMoves}");

            return res.ToString();
        }
    }
}
