namespace Support
{
    using Common;
    using Models;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class TcpHelper
    {
        private readonly int port;
        private readonly IPAddress ipAddress;

        private TcpClient Client { get; set; }
        private TcpListener Server { get; set; }
        private Func<string, TcpResponse> RequestCallback;

        public TcpHelper(int port)
        {
            this.port = port;
            this.ipAddress = IPAddress.Parse("127.0.0.1");
        }

        public async Task StartServerAsync(Func<string, TcpResponse> requestCallback)
        {
            this.RequestCallback = requestCallback;
            this.Server = new TcpListener(this.ipAddress, this.port);

            this.Server.Start();

            ConsoleHelper.Log($"Server started. Listening to TCP clients at 127.0.0.1:{port}");

            try
            {
                ConsoleHelper.Log("Waiting for client...");

                while (true)
                {
                    Task<TcpClient> task = this.Server.AcceptTcpClientAsync();

                    if (task.Result != null)
                    {
                        TcpClient client = task.Result;
                        ConsoleHelper.Log($"Connected {client.Client.RemoteEndPoint}");
                        await this.Listen(client);
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleHelper.Log(e.ToString());
            }
            finally
            {
                this.Server.Stop();
            }
        }

        public void StartClient()
        {
            this.Client = new TcpClient();

            this.Client.Client.Connect(this.ipAddress, this.port);

            ConsoleHelper.Log($"Connected to {this.ipAddress}:{this.port}");
        }

        public Task Send(RequestModel request, Func<string, Task> ResponseCallback)
        {
            if (!this.Client.Connected)
            {
                this.StartClient(); 
            }

            this.Client.Client.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request)));

            Task result = ReadDataLoop(this.Client, ResponseCallback);

            return result;
        }

        private async Task Listen(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    ActionState state = ActionState.Continue;

                    while (state == ActionState.Continue)
                    {
                        string message 
                            = this.ReadData(client);

                        ConsoleHelper.LogRequest(message);

                        TcpResponse response
                            = RequestCallback(message);

                        string responseMessage = response.Message;
                        state = response.State;

                        if (responseMessage != string.Empty)
                        {
                            byte[] send_data = Encoding.ASCII.GetBytes(responseMessage);

                            await stream.WriteAsync(send_data, 0, send_data.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private Task ReadDataLoop(TcpClient client, Func<string, Task> ResponseCallback)
        {
            while (true)
            {
                string response = ReadData(client);

                return ResponseCallback(response);

                // left if i want to implement continuous response
                if (response == string.Empty
                    && !client.Connected)
                {
                    return Task.CompletedTask;
                }
            }
        }

        private string ReadData(TcpClient client)
        {
            string message = string.Empty;
            try
            {
                NetworkStream stream = client.GetStream();

                if (client.ReceiveBufferSize > 0)
                {
                    byte[] bytes = new byte[client.ReceiveBufferSize];

                    int numberOfBytes
                        = stream.Read(bytes, 0, client.ReceiveBufferSize);

                    message
                        = Encoding.ASCII.GetString(bytes, 0, numberOfBytes);
                }

                return message;
            }
            catch (Exception e)
            {
                ConsoleHelper.Log(e.ToString());
                return message;
            }
        }
    }
}
