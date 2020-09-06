namespace EpsServer
{
    using System;
    using Models;
    using Newtonsoft.Json;
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
#if DEBUG
                case "check-service":
                    result.Message = "Connection successful";
                    break;
#endif  
                case "check":
                    CheckDiscountRequestModel checkRequest = default;

                    if (this.TryDeserialize(command,out checkRequest))
                    {
                        //check logic
                    }
                    else
                    {
                        result.Message = "Invalid Request";
                    }

                    break;
                case "usecode":
                    UseDiscountCodeRequestModel useCodeRequest = default;

                    if (this.TryDeserialize(command, out useCodeRequest))
                    {
                        //use code logic
                    }
                    else
                    {
                        result.Message = "Invalid Request";
                    }
                    break;
                default:
                    result.Message = "Incorrect command";
                    break;
            }

            return result;
        }

        private bool TryDeserialize<T>(string message, out T obj)
        {
            bool result = false;

            try
            {
                obj = JsonConvert.DeserializeObject<T>(message);

                result = true;

                return result;
            }
            catch (Exception e)
            {
                ConsoleHelper.Log(e.ToString());
                obj = default;
                return result;
            }
        }
    }
}
