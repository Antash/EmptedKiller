using System;
using System.IO;

namespace EmptedKiller
{
    class Program
    {
        static Position pos;
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
                    pos = new Position(true);
                    break;
                case UCICommandType.PositionNextMove:
                    if (pos == null)
                    {
                        pos = new Position(false);
                    }
                    pos.MakeMove(args.Data);
                    break;
                case UCICommandType.Go:
                    var bestMove = pos.GetNextMove();
                    pos.MakeMove(bestMove);
                    using (var commandLog = new StreamWriter(@"C:\Users\aashm\Downloads\log.txt", true))
                    {
                        commandLog.WriteLine($"Current position: {pos.ToString()}");
                        commandLog.WriteLine($"bestmove {bestMove}");
                    }
                    Console.WriteLine($"bestmove {bestMove}");
                    break;
                case UCICommandType.Quit:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
