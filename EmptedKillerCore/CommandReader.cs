using System;
using System.IO;
using System.Linq;

namespace EmptedKillerCore
{
    public class CommandReader
    {
        public delegate void UCICommandRecievedHandler(UCICommandEventArgs args);

        public event UCICommandRecievedHandler CommandRecieved;

        public void Run()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    using (var commandLog = new StreamWriter(@"C:\Users\aashm\Downloads\log.txt", true))
                    {
                        commandLog.WriteLine(input);
                    }
                    input = input.ToLower();
                    var commandChunks = input.Split(null);
                    var command = commandChunks[0];
                    switch (command)
                    {
                        case "uci":
                            OnCommandRecieved(new UCICommandEventArgs(UCICommandType.UCI));
                            break;
                        case "debug":
                            break;
                        case "isready":
                            OnCommandRecieved(new UCICommandEventArgs(UCICommandType.IsReady));
                            break;
                        case "setoption":
                            break;
                        case "register":
                            break;
                        case "ucinewgame":
                            OnCommandRecieved(new UCICommandEventArgs(UCICommandType.UCINewGame));
                            break;
                        case "position":
                            if (commandChunks[1] == "startpos")
                            {
                                // In game mode position always sent with a set of moves made
                                if (commandChunks.Length > 2 && commandChunks[2] == "moves")
                                {
                                    var lastMove = commandChunks.Last();
                                    OnCommandRecieved(new UCICommandEventArgs(UCICommandType.PositionNextMove) { Data = lastMove });
                                }
                                else
                                {
                                    OnCommandRecieved(new UCICommandEventArgs(UCICommandType.Position));
                                }
                            }
                            else
                            {
                                OnCommandRecieved(new UCICommandEventArgs(UCICommandType.Position));
                            }
                            break;
                        case "go":
                            OnCommandRecieved(new UCICommandEventArgs(UCICommandType.Go));
                            break;
                        case "stop":
                            break;
                        case "ponderhit":
                            break;
                        case "quit":
                            OnCommandRecieved(new UCICommandEventArgs(UCICommandType.Quit));
                            break;
                    }
                }
            }
        }

        private void OnCommandRecieved(UCICommandEventArgs args)
        {
            var handler = CommandRecieved;
            handler?.Invoke(args);
        }
    }
}
