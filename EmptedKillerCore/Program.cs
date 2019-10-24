﻿using System;
using System.IO;
using System.Linq;

namespace EmptedKillerCore
{
    class Program
    {
        static IPosition pos;
        static FenSerializer<NaivePositionBuilder> fs = new FenSerializer<NaivePositionBuilder>();
        static IEvaluate ev = new EvaluationCore();
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
                    var eng = new Engine(ev);
                    eng.Analyze(pos);

                    var ll = eng.lines;
                    var maxEval = ll.Max(la => ev.Evaluate(la));
                    var best = ll.Where(l => Math.Abs(ev.Evaluate(l) - maxEval) < float.Epsilon).ToList();
                    var theBest = best[rnd.Next(best.Count)];
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