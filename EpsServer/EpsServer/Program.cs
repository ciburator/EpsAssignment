namespace EpsServer
{
    using System;
    using Support;
    using Support.Models;

    class Program
    {
        static void Main(string[] args)
        {
            new Program().Initialize();

            AppDomain.CurrentDomain.ProcessExit += ProcessExitEventHandler;
        }

        private static void ProcessExitEventHandler(object sender, EventArgs e)
        {
        }

        public void Initialize()
        {
            var helper = new TcpHelper(8181);
             helper.StartServerAsync(RequestCallback).ConfigureAwait(true);
        }

        public TcpResponse RequestCallback(string command)
        {
            TcpResponse result = new TcpResponse();

            switch (command.ToLower())
            {
                case "check":
                    result.Message = "Connection successful";
                    break;
                default:
                    result.Message = "Incorrect command";
                    break;
            }

            return result;
        }
    }
}
