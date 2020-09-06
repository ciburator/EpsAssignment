namespace Support
{
    using System;

    public class ConsoleHelper
    {
        public static string RequestCommand()
        {
            Console.Write("input command: ");
            return Console.ReadLine();
        }

        public static void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now} : {message}");
        }

        public static void LogRequest(string message)
        {
            Console.WriteLine($"{DateTime.Now} Request message: {message}");
        }

        public static void LogResponse(string message)
        {
            Console.WriteLine($"{DateTime.Now} Response message: {message}");
        }
    }
}
