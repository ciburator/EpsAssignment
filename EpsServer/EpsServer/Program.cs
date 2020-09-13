namespace EpsServer
{
    using Newtonsoft.Json;
    using Support;
    using Support.Models;
    using System;
    using System.Collections.Generic;
    using Database;
    using Database.Interfaces;
    using Database.Models;
    using Models;
    using Services;

    class Program
    {
        private IDatabaseHandler DbHandler { get; set; }

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
            this.DbHandler = new DatabaseHandler();
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

                        if (this.TryDeserialize(request.Request, out CheckDiscountRequestModel checkRequest))
                        {
                            DiscountModel discount 
                                = this.DbHandler.CheckDiscountCode(checkRequest.CardNumber);

                            result.Message
                                = JsonConvert.SerializeObject(
                                    new CheckDiscountResponseModel(discount.IsUsed, discount.ProductCodes));
                        }
                        else
                        {
                            result.Message = "Invalid Request";
                        }

                        break;

                    case "usecode":

                        if (this.TryDeserialize(request.Request, out UseDiscountCodeRequestModel useCodeRequest))
                        {
                            result.Message
                                = JsonConvert.SerializeObject(new UseDiscountCodeResponseModel(this.DbHandler.UseCode(useCodeRequest.Code)));
                        }
                        else
                        {
                            result.Message = "Invalid Request";
                        }

                        break;

                    case "generate":

                        if (this.TryDeserialize(request.Request, out GeneratedDiscountRequestModel generatedDiscountRequest)
                            && this.DbHandler.CheckProduct(generatedDiscountRequest.Products))
                        {
                            this.DbHandler.GenerateCodes(new CodeGenerator().Generate(2000, 8), generatedDiscountRequest.Products);

                            result.Message = $" 2000 Codes generated";
                        }
                        else
                        {
                            result.Message = "Invalid Request";
                        }
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
