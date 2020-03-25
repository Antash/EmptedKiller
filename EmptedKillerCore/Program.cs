using EmptedKillerCore.Engine;
using EmptedKillerCore.Evaluation;
using System;
using System.IO;

namespace EmptedKillerCore
{
    class Program
    {
        static IPosition pos;
        static FenSerializer<NaivePositionBuilder> fs = new FenSerializer<NaivePositionBuilder>();
        static IEvaluate ev = new EvaluationCore(
            new PieceValueEvaluation(), 
            new PieceActivityEvaluation()
            );
        static Random rnd = new Random(DateTime.Now.Millisecond);
        static void Main(string[] args)
        {
            using (var commandLog = new StreamWriter(@"C:\Users\aashm\Downloads\log.txt"))
            {
                commandLog.WriteLine($"New game: {DateTime.Now}");
            }
            var inputReader = new CommandReader();
            inputReader.CommandRecieved += InputReader_CommandRecieved;
            inputReader.Run();
        }

        private static void InputReader_CommandRecieved(UCICommandEventArgs args)
        {
            switch(args.Command)
            {
                case UCICommandType.UCI:
                    Console.WriteLine("id name emptedkiller");
                    Console.WriteLine("id author antash");
                    Console.WriteLine("uciok");
                    break;
                case UCICommandType.IsReady:
                    Console.WriteLine("readyok");
                    break;
                case UCICommandType.UCINewGame:
                    pos = null;
                    break;
                case UCICommandType.Position:
                    pos = new NaivePosition();
                    break;
                case UCICommandType.PositionNextMove:
                    if (pos == null)
                    {
                        pos = new NaivePosition();
                    }
                    pos = pos.MakeMove(NotationHelper.ParseMoveCode(args.Data));
                    break;
                case UCICommandType.Go:
                    var eng = new EngineRecursive(ev);
                    eng.Analyze(pos);

                    var ll = eng.lines;
                    var theBest = ll[rnd.Next(ll.Count)];
                    var bestMove = theBest.Moves[pos.Moves.Count];
                    pos = pos.MakeMove(bestMove);
                    var bestmoveStr = NotationHelper.GetMoveCode(bestMove);
                    using (var commandLog = new StreamWriter(@"C:\Users\aashm\Downloads\log.txt", true))
                    {
                        commandLog.WriteLine($"Current position: {fs.Write(pos)}");
                        commandLog.WriteLine($"bestmove {bestmoveStr}");
                    }
                    Console.WriteLine($"bestmove {bestmoveStr}");
                    break;
                case UCICommandType.Quit:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
