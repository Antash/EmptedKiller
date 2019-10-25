using System;
using System.Text;

namespace EmptedKillerCore
{
    public class FenSerializer<TBuilder> : IPositionSerializer 
        where TBuilder : IPositionBuilder, new()
    {
        public IPosition Read(string fenString)
        {
            var builder = new TBuilder();
            try
            {
                var chunks = fenString.Split();
                var boardChunks = chunks[0].Split(new[] { '/' });

                for (int rank = 0; rank < 8; rank++)
                {
                    var rankChunk = boardChunks[rank];
                    int file = 0;
                    foreach (var item in rankChunk)
                    {
                        if (int.TryParse(item.ToString(), out var gap))
                        {
                            file += gap;
                        }
                        else
                        {
                            var piece = NotationHelper.GetPiece(item);
                            builder.PutPiece(rank, file, piece | (char.IsLower(item) ? Piece.Black : Piece.None));
                            file++;
                        }
                    }
                }
                builder.SetWhiteToMove(chunks[1].ToLower() == "w");
                if (chunks[2] != "-")
                {
                    var flags = Castling.None;
                    flags |= chunks[2].Contains('Q') ? Castling.QueenSide : Castling.None;
                    flags |= chunks[2].Contains('K') ? Castling.KingSide : Castling.None;
                    builder.SetCastling(true, flags);
                    flags = Castling.None;
                    flags |= chunks[2].Contains('q') ? Castling.QueenSide : Castling.None;
                    flags |= chunks[2].Contains('k') ? Castling.KingSide : Castling.None;
                    builder.SetCastling(false, flags);
                }
                if (chunks[3] != "-")
                {
                    builder.SetEnPassant(7 - (chunks[3][1] - '1'), chunks[3][0] - 'a');
                }
                if (chunks.Length > 4)
                {
                    builder.SetHalfMoves(int.Parse(chunks[4]));
                    builder.SetFullMoves(int.Parse(chunks[5]));
                }
                return builder.Build();
            }
            catch
            {
                throw new FormatException($"Wrong fen string: {fenString}");
            }
        }

        public string Write(IPosition position)
        {
            var res = new StringBuilder();
            for (int rank = 0; rank < 8; rank++)
            {
                int gap = 0;
                for (int file = 0; file < 8; file++)
                {
                    var piece = position.GetPiece(rank, file);
                    if (piece != Piece.None)
                    {
                        char pieceChar = NotationHelper.GetPieceChar(piece);
                        pieceChar = (piece & Piece.Black) == 0 ? char.ToUpper(pieceChar) : pieceChar;
                        if (gap > 0)
                        {
                            res.Append(gap);
                            gap = 0;
                        }
                        res.Append(pieceChar);
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

            res.Append($" {(position.WhiteToMove ? 'w' : 'b')}");

            if (position.WhiteCastling != Castling.None || position.BlackCastling != Castling.None)
            {
                res.Append(" ");
                if ((position.WhiteCastling & Castling.KingSide) != 0)
                {
                    res.Append('K');
                }
                if ((position.WhiteCastling & Castling.QueenSide) != 0)
                {
                    res.Append('Q');
                }
                if ((position.BlackCastling & Castling.KingSide) != 0)
                {
                    res.Append('k');
                }
                if ((position.BlackCastling & Castling.QueenSide) != 0)
                {
                    res.Append('q');
                }
            }
            else
            {
                res.Append(" -");
            }

            if (position.EnPassant)
            {
                res.Append($" {(char)('a' + position.EnPassantFile)}{8 - position.EnPassantRank}");
            }
            else
            {
                res.Append(" -");
            }

            res.Append($" {position.HalfMoveClock}");
            res.Append($" {position.TotalMoves}");

            return res.ToString();
        }
    }
}
