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
    }
}
