namespace EpsServer
{
    using Newtonsoft.Json;
    using Support;
    using Support.Models;
    using System;
    using Models;
    using Services;

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

            if (TryDeserialize(command, out RequestModel request))
            {
                switch (request.Command.ToLower())
                {
#if DEBUG
                    case "check-service":
                        result.Message = "Connection successful";
                        break;
#endif
                    case "check":

                        if (this.TryDeserialize(command, out CheckDiscountRequestModel checkRequest))
                        {
                            result.Message
                                = JsonConvert.SerializeObject(
                                    new CheckDiscountResponseModel(1, new[] {"test", "test1"}));
                        }
                        else
                        {
                            result.Message = "Invalid Request";
                        }

                        break;

                    case "usecode":

                        if (this.TryDeserialize(command, out UseDiscountCodeRequestModel useCodeRequest))
                        {
                            result.Message
                                = JsonConvert.SerializeObject(new UseDiscountCodeResponseModel(1));
                        }
                        else
                        {
                            result.Message = "Invalid Request";
                        }

                        break;

                    case "generate":

                        var codes = new CodeGenerator().Generate(2000, 8);

                        result.Message = $" 2000 Codes generated";
                        break;

                    default:
                        result.Message = "Invalid command";
                        break;
                }
            }
            else
            {
                result.Message = "Invalid command";
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
