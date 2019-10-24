namespace EmptedKillerCore
{
    public class UCICommandEventArgs
    {
        public UCICommandType Command { get;  }

        public string Data { get; internal set; }

        public UCICommandEventArgs(UCICommandType command)
        {
            Command = command;
        }
    }
}
