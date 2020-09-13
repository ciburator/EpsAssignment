namespace EpsClient
{
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Newtonsoft.Json;
    using Support;
    using Support.Models;

    class Program
    {
        static void Main(string[] args)
        {
            new Program().Initialize();
        }

        public void Initialize()
        {
            var helper = new TcpHelper(Config.Port);

            helper.StartClient();

            while (true)
            {
                RequestModel request
                    = this.HandleCommand(ConsoleHelper.RequestCommand());

                if (request != null)
                {
                    helper.Send(request, this.HandleResponse);
                }
                else
                {
                    ConsoleHelper.Log("Invalid command");
                }
            }
        }

        private RequestModel HandleCommand(string command)
        {
            RequestModel result = new RequestModel();

            result.Command = command;

            switch (command)
            {
#if DEBUG
                case "check-service":
                    break;

                case "select":
                    break;

                case "generate":
                    string products 
                        = ConsoleHelper.RequestInput("Please input products separated by comma (',')");

                    if (!string.IsNullOrEmpty(products))
                    {
                        string[] items = products.Split(',')
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .ToArray();

                        result.Request
                            = JsonConvert.SerializeObject(new GeneratedDiscountRequestModel(items));
                    }
                    else
                    {
                        ConsoleHelper.Log("Invalid input");
                    }
                    break;
#endif
                case "check":
                    string cardNumber
                        = ConsoleHelper.RequestInput(nameof(CheckDiscountRequestModel.CardNumber));

                    result.Request
                        = JsonConvert.SerializeObject(new CheckDiscountRequestModel(cardNumber));
                    break;
                case "useCode":
                    string code
                        = ConsoleHelper.RequestInput(nameof(UseDiscountCodeRequestModel.Code));

                    result.Request
                        = JsonConvert.SerializeObject(new UseDiscountCodeRequestModel(code));
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        private Task HandleResponse(string response)
        {
            ConsoleHelper.Log(response);

            return Task.CompletedTask;
        }
    }
}
