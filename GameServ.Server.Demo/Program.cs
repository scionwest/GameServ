using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GameServ.Server.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            bool benchmark = false;
            if (args.Contains("--benchmark"))
            {
                benchmark = true;
            }

            var server = new Server();
            int count = 0;
            int lastPostedCount = 0;
            int currentlyPostedCount = 0;

            var watch = new Stopwatch();
            bool isRunning = false;
            if (benchmark)
            {
                var benchmarkTimer = new ServerTimer<Server>(server);
                server.ClientDatagramReceived += datagram =>
                {
                    if (!isRunning)
                    {
                        isRunning = true;
                        benchmarkTimer.Start(1000, 1000, 0, (runningServer, timer) =>
                        {
                            lastPostedCount = currentlyPostedCount;
                            currentlyPostedCount = count;
                            count = 0;
                            Console.Write("\rAverage Requests per second: {0}            ", currentlyPostedCount);
                        });
                        watch.Start();
                    }

                    count++;
                };
            }

            server.Start();
            Console.WriteLine($"Server running at {server.serverEndPoint}:{server.ServerPort}");
            while (server.IsRunning)
            {
                Task.Delay(1).Wait();
            }
        }
    }
}
