namespace EpsClient
{
    using Support;

    class Program
    {
        static void Main(string[] args)
        {
            new Program().Initialize();
        }

        public void Initialize()
        {
            var helper = new TcpHelper(8181);

            helper.StartClient();
        }
    }
}
