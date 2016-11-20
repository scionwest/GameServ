using System;
using System.Threading.Tasks;

namespace GameServ.Server.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var ipAddress = new byte[] { 10, 0, 1, 6 };
            IServer server = new Server(ipAddress);
            server.Start();

            Console.WriteLine($"Server running at {server.GetAvailableServerEndPoint()}");
            while (server.IsRunning)
            {
                Task.Delay(1).Wait();
            }

            server.Shutdown();
        }
    }
}
