namespace Support
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Models;

    public class TcpHelper
    {
        private int port;
        private IPAddress ipAddress;
        private bool accept { get; set; } = false;

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

            accept = true;

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

            

            string command = string.Empty;

            while (command != "exit")
            {
                command = ConsoleHelper.RequestCommand();

                if (command != string.Empty)
                {
                    this.Client.Client.Send(Encoding.ASCII.GetBytes(command));

                    Task result = Task.Run(() => ReadDataLoopAsync(this.Client));

                    result.Wait();
                }
            }
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
                        byte[] bytes;
                        StringBuilder myCompleteMessage = new StringBuilder();
                        string message = string.Empty;

                        if (client.ReceiveBufferSize > 0)
                        {
                            bytes = new byte[client.ReceiveBufferSize];
                            int numberOfBytes = stream.Read(bytes, 0, client.ReceiveBufferSize);
                            message = Encoding.ASCII.GetString(bytes, 0, numberOfBytes);
                        }

                        ConsoleHelper.LogRequest(message);

                        TcpResponse response
                            = RequestCallback(message);

                        string responseMessage = response.Message;
                        state = response.State;

                        if (responseMessage != string.Empty)
                        {
                            byte[] send_data = Encoding.ASCII.GetBytes(responseMessage);

                            await stream.WriteAsync(send_data, 0, send_data.Length);

                            message = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task ReadDataLoopAsync(TcpClient helper)
        {
            while (true)
            {
                if (!helper.Connected)
                    break;

                string response = string.Empty;
                response = ReadData(helper);
                ConsoleHelper.LogResponse(response);

                // left if i want to implement continuous response
                if (response != string.Empty)
                {
                    break;
                }
            }
        }

        private string ReadData(TcpClient helper)
        {
            string result = string.Empty;

            byte[] data = new byte[1024];

            NetworkStream stream = helper.GetStream();

            byte[] myReadBuffer = new byte[1024];

            StringBuilder myCompleteMessage = new StringBuilder();

            int numberOfBytesRead = 0;

            do
            {
                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);

                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

            }
            while (stream.DataAvailable);

            result = myCompleteMessage.ToString();

            return result;
        }
    }
}
