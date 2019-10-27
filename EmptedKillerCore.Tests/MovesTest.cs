using EmptedKillerCore;
using NUnit.Framework;
using System.Linq;

namespace EmptedKillerCoreTests
{
    public class MovesTest
    {
        private readonly FenSerializer<NaivePositionBuilder> _reader = new FenSerializer<NaivePositionBuilder>();

        [Test]
        public void PawnGeneralMovesWhite()
        {
            string fen = "8/p7/8/6p1/2P3Pp/8/3P4/8 w - - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves().Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "c4c5", "d2d4", "d2d3" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("c4c5"));
            Assert.AreEqual(Piece.None, pos1.GetPiece(4, 2));
            Assert.AreEqual(Piece.Pawn, pos1.GetPiece(3, 2));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("d2d4"));
            Assert.AreEqual(Piece.None, pos2.GetPiece(6, 3));
            Assert.AreEqual(Piece.Pawn, pos2.GetPiece(4, 3));
            Assert.IsTrue(pos2.EnPassant);
            Assert.AreEqual(pos2.EnPassantFile, 3);
            Assert.AreEqual(pos2.EnPassantRank, 5);
        }

        [Test]
        public void PawnGeneralMovesBlack()
        {
            string fen = "8/p7/8/6p1/2P3Pp/8/3P4/8 b - - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves().Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "h4h3", "a7a5", "a7a6" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("h4h3"));
            Assert.AreEqual(Piece.None, pos1.GetPiece(4, 7));
            Assert.AreEqual(Piece.Pawn | Piece.Black, pos1.GetPiece(5, 7));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("a7a5"));
            Assert.AreEqual(Piece.None, pos2.GetPiece(1, 0));
            Assert.AreEqual(Piece.Pawn | Piece.Black, pos2.GetPiece(3, 0));
            Assert.IsTrue(pos2.EnPassant);
            Assert.AreEqual(pos2.EnPassantFile, 0);
            Assert.AreEqual(pos2.EnPassantRank, 2);
        }

        [Test]
        public void PawnCapturesWhite()
        {
            string fen = "8/8/4p3/ppp1Pp2/1P6/8/8/8 w - f6 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves().Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "b4a5", "b4c5", "e5f6" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("b4a5"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("b4".GetRank(), "b4".GetFile()));
            Assert.AreEqual(Piece.Pawn, pos1.GetPiece("a5".GetRank(), "a5".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("e5f6"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("e5".GetRank(), "e5".GetFile()));
            Assert.AreEqual(Piece.Pawn, pos2.GetPiece("f6".GetRank(), "f6".GetFile()));
            Assert.IsFalse(pos2.EnPassant);
        }

        [Test]
        public void PawnCapturesBlack()
        {
            string fen = "8/8/8/5p2/1Pp1PPP1/2P5/8/8 b - b3 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves().Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "f5e4", "f5g4", "c4b3" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("f5g4"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("f5".GetRank(), "f5".GetFile()));
            Assert.AreEqual(Piece.Pawn | Piece.Black, pos1.GetPiece("g4".GetRank(), "g4".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("c4b3"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("c4".GetRank(), "c4".GetFile()));
            Assert.AreEqual(Piece.Pawn | Piece.Black, pos2.GetPiece("b3".GetRank(), "b3".GetFile()));
            Assert.IsFalse(pos2.EnPassant);
        }

        [Test]
        public void PawnPromotionsBlack()
        {
            string fen = "8/3P4/8/8/8/8/4p3/8 b - - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves().Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "e2e1q", "e2e1n", "e2e1b", "e2e1r" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("e2e1n"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("e2".GetRank(), "e2".GetFile()));
            Assert.AreEqual(Piece.Knight | Piece.Black, pos1.GetPiece("e1".GetRank(), "e1".GetFile()));
        }

        [Test]
        public void PawnPromotionsWhite()
        {
            string fen = "8/3P4/8/8/8/8/4p3/8 w - - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves().Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "d7d8q", "d7d8n", "d7d8b", "d7d8r" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("d7d8q"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("d7".GetRank(), "d7".GetFile()));
            Assert.AreEqual(Piece.Queen, pos1.GetPiece("d8".GetRank(), "d8".GetFile()));
        }

        [Test]
        public void KnightMovesAndCapturesWhite()
        {
            string fen = "8/8/8/5p2/1p1N1P2/1P2p3/4P3/N7 w - - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves().Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "a1c2", "d4c2", "d4b5", "d4c6", "d4e6", "d4f5", "d4f3" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("a1c2"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("a1".GetRank(), "a1".GetFile()));
            Assert.AreEqual(Piece.Knight, pos1.GetPiece("c2".GetRank(), "c2".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("d4f5"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("d4".GetRank(), "d4".GetFile()));
            Assert.AreEqual(Piece.Knight, pos2.GetPiece("f5".GetRank(), "f5".GetFile()));
        }

        [Test]
        public void KnightMovesAndCapturesBlack()
        {
            string fen = "n7/2p5/1pn5/1P6/1P1p4/3P4/8/8 b - - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves().Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "c6a5", "c6a7", "c6b8", "c6d8", "c6b4", "c6e7", "c6e5" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("c6a5"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("c6".GetRank(), "c6".GetFile()));
            Assert.AreEqual(Piece.Knight | Piece.Black, pos1.GetPiece("a5".GetRank(), "a5".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("c6b4"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("c6".GetRank(), "c6".GetFile()));
            Assert.AreEqual(Piece.Knight | Piece.Black, pos2.GetPiece("b4".GetRank(), "b4".GetFile()));
        }

        [Test]
        public void BishopMovesAndCapturesWhite()
        {
            string fen = "r1bqkbnr/ppp1pppp/n7/3p4/2B1P3/8/PPPPNPPP/RNBQK2R w KQkq - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves("c4".GetRank(), "c4".GetFile()).Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "c4b5", "c4a6", "c4b3", "c4d3", "c4d5" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("c4a6"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("c4".GetRank(), "c4".GetFile()));
            Assert.AreEqual(Piece.Bishop, pos1.GetPiece("a6".GetRank(), "a6".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("c4b5"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("c4".GetRank(), "c4".GetFile()));
            Assert.AreEqual(Piece.Bishop, pos2.GetPiece("b5".GetRank(), "b5".GetFile()));
        }

        [Test]
        public void BishopMovesAndCapturesBlack()
        {
            string fen = "rn1qkbnr/ppp1p1pp/3p4/5p2/8/6Pb/PPPPPP1P/RNBQKBNR b KQkq - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves("h3".GetRank(), "h3".GetFile()).Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "h3g2", "h3f1", "h3g4" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("h3g4"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("h3".GetRank(), "h3".GetFile()));
            Assert.AreEqual(Piece.Bishop | Piece.Black, pos1.GetPiece("g4".GetRank(), "g4".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("h3f1"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("h3".GetRank(), "h3".GetFile()));
            Assert.AreEqual(Piece.Bishop | Piece.Black, pos2.GetPiece("f1".GetRank(), "f1".GetFile()));
        }

        [Test]
        public void RookMovesAndCapturesWhite()
        {
            string fen = "rnbqkbnr/pppppppp/7P/8/4P1R1/8/PPPP1PP1/RNBQKBN1 w Qkq - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves("g4".GetRank(), "g4".GetFile()).Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "g4g5", "g4g6", "g4g7", "g4h4", "g4f4", "g4g3" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("g4g7"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("g4".GetRank(), "g4".GetFile()));
            Assert.AreEqual(Piece.Rook, pos1.GetPiece("g7".GetRank(), "g7".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("g4f4"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("g4".GetRank(), "g4".GetFile()));
            Assert.AreEqual(Piece.Rook, pos2.GetPiece("f4".GetRank(), "f4".GetFile()));
        }

        [Test]
        public void RookMovesAndCapturesBlack()
        {
            string fen = "rn1qkbn1/p1p1ppp1/3p4/8/1p1r2b1/8/PPPPPPPP/RNBQKBNR b KQq - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves("d4".GetRank(), "d4".GetFile()).Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "d4d5", "d4d3", "d4d2", "d4c4", "d4e4", "d4f4" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("d4d2"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("d4".GetRank(), "d4".GetFile()));
            Assert.AreEqual(Piece.Rook | Piece.Black, pos1.GetPiece("d2".GetRank(), "d2".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("d4f4"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("d4".GetRank(), "d4".GetFile()));
            Assert.AreEqual(Piece.Rook | Piece.Black, pos2.GetPiece("f4".GetRank(), "f4".GetFile()));
        }

        [Test]
        public void QueenMovesAndCapturesWhite()
        {
            string fen = "rnbqkb1r/p1pppppp/8/8/1p2Pn2/3P4/PPPQ1PPP/RNB1KBNR w KQkq - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves("d2".GetRank(), "d2".GetFile()).Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "d2d1", "d2c3", "d2b4", "d2e2", "d2e3", "d2f4" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("d2d1"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("d2".GetRank(), "d2".GetFile()));
            Assert.AreEqual(Piece.Queen, pos1.GetPiece("d1".GetRank(), "d1".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("d2f4"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("d2".GetRank(), "d2".GetFile()));
            Assert.AreEqual(Piece.Queen, pos2.GetPiece("f4".GetRank(), "f4".GetFile()));
        }

        [Test]
        public void QueenMovesAndCapturesBlack()
        {
            string fen = "rnbk1bnr/p1p2ppp/1p1q1N2/2Ppp3/8/8/PP1PPPPP/R1BQKBNR b KQ - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves("d6".GetRank(), "d6".GetFile()).Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "d6d7", "d6c5", "d6c6", "d6e7", "d6e6", "d6f6" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("d6c6"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("d6".GetRank(), "d6".GetFile()));
            Assert.AreEqual(Piece.Queen | Piece.Black, pos1.GetPiece("c6".GetRank(), "c6".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("d6f6"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("d6".GetRank(), "d6".GetFile()));
            Assert.AreEqual(Piece.Queen | Piece.Black, pos2.GetPiece("f6".GetRank(), "f6".GetFile()));
        }

        [Test]
        public void KingMovesAndCapturesWhite()
        {
            string fen = "rnbqkb1r/pppp1ppp/8/8/4p3/3PKn2/PPPP1PPP/RNBQ1BNR w kq - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves("e3".GetRank(), "e3".GetFile()).Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "e3d4", "e3e4", "e3f4", "e3f3", "e3e2" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("e3d4"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("e3".GetRank(), "e3".GetFile()));
            Assert.AreEqual(Piece.King, pos1.GetPiece("d4".GetRank(), "d4".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("e3e4"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("e3".GetRank(), "e3".GetFile()));
            Assert.AreEqual(Piece.King, pos2.GetPiece("e4".GetRank(), "e4".GetFile()));
        }

        [Test]
        public void KingMovesAndCapturesBlack()
        {
            string fen = "rnbq1b1r/ppp3pp/3p1n2/4kp2/4pP2/8/PPPPP1PP/RNBQKBNR b KQ - 0 1";
            var position = _reader.Read(fen);
            var moves = position.GetValidMoves("e5".GetRank(), "e5".GetFile()).Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "e5d4", "e5d5", "e5f4", "e5e6" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("e5d4"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("e5".GetRank(), "e5".GetFile()));
            Assert.AreEqual(Piece.King | Piece.Black, pos1.GetPiece("d4".GetRank(), "d4".GetFile()));

            var pos2 = position.MakeMove(NotationHelper.ParseMoveCode("e5f4"));
            Assert.AreEqual(Piece.None, pos2.GetPiece("e5".GetRank(), "e5".GetFile()));
            Assert.AreEqual(Piece.King | Piece.Black, pos2.GetPiece("f4".GetRank(), "f4".GetFile()));
        }

        [Test]
        public void KingCastlingWhite()
        {
            string fen = "1nbqkbn1/ppppppp1/8/5r2/8/8/PP2P1PP/R3K2R w KQ - 0 1";
            var position = _reader.Read(fen);

            Assert.AreEqual(Castling.KingSide | Castling.QueenSide, position.WhiteCastling);

            var moves = position.GetValidMoves("e1".GetRank(), "e1".GetFile()).Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "e1f1", "e1f2", "e1d2", "e1d1", "e1c1" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("e1c1"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("e1".GetRank(), "e1".GetFile()));
            Assert.AreEqual(Piece.None, pos1.GetPiece("a1".GetRank(), "a1".GetFile()));
            Assert.AreEqual(Piece.King, pos1.GetPiece("c1".GetRank(), "c1".GetFile()));
            Assert.AreEqual(Piece.Rook, pos1.GetPiece("d1".GetRank(), "d1".GetFile()));
            Assert.AreEqual(Castling.None, pos1.WhiteCastling);
        }

        [Test]
        public void KingCastlingBlack()
        {
            string fen = "r3k2r/pp1ppppp/8/B7/8/8/PPP1PPPP/RN1QKBNR b KQkq - 0 1";
            var position = _reader.Read(fen);

            Assert.AreEqual(Castling.KingSide | Castling.QueenSide, position.BlackCastling);

            var moves = position.GetValidMoves("e8".GetRank(), "e8".GetFile()).Select(m => NotationHelper.GetMoveCode(m));
            Assert.That(moves, Is.EquivalentTo(new[] { "e8f8", "e8d8", "e8g8" }));

            var pos1 = position.MakeMove(NotationHelper.ParseMoveCode("e8g8"));
            Assert.AreEqual(Piece.None, pos1.GetPiece("e8".GetRank(), "e8".GetFile()));
            Assert.AreEqual(Piece.None, pos1.GetPiece("h8".GetRank(), "h8".GetFile()));
            Assert.AreEqual(Piece.King | Piece.Black, pos1.GetPiece("g8".GetRank(), "g8".GetFile()));
            Assert.AreEqual(Piece.Rook | Piece.Black, pos1.GetPiece("f8".GetRank(), "f8".GetFile()));
            Assert.AreEqual(Castling.None, pos1.BlackCastling);
        }
    }
}
