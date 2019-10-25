using EmptedKillerCore;
using NUnit.Framework;

namespace EmptedKillerCoreTests
{
    public class FenSerializerTest
    {
        private readonly FenSerializer<NaivePositionBuilder> _serializer = new FenSerializer<NaivePositionBuilder>();

        [Test]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
        [TestCase("R7/1p4pk/2r1p2p/8/1q2nP2/3Q4/1P4PP/1N5K w - - 5 31")]
        [TestCase("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2")]       
        public void CanReadWritePosition(string fen)
        {
            var position = _serializer.Read(fen);
            var savedFen = _serializer.Write(position);
            Assert.AreEqual(fen, savedFen, "FEN strings do not match");
        }

        [Test]
        [TestCase("r1b1kbnr/pppn1ppp/3p2q1/4p3/1P1B3P/P4P2/2PPP1P1/RN1QKBNR w KQkq -")]
        [TestCase("r2k2n1/pp1b2br/5qNp/1B1Q2p1/8/4P3/PPP2PPP/R3K2R w KQ -")]
        public void CanReadNotFullPositionFen(string fen)
        {
            var position = _serializer.Read(fen);
            var savedFen = _serializer.Write(position);
            Assert.IsTrue(savedFen.StartsWith(fen), "FEN strings do not match");
        }
    }
}