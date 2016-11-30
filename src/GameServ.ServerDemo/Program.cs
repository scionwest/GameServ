using GameServ;
using GameServ.Core;
using GameServ.Datagrams.DatagramMessages;
using GameServ.Server;
using System;
using System.Net;

class Program
{
    static void Main(string[] args)
    {
        DatagramFactory datagramFactory = null;

        MessageBroker.Default.Subscribe<DatagramReceivedMessage>(
            (msg, sub) =>
            {
                if (msg.Header.MessageType == 2)
                {
                    Console.WriteLine($"Heartbeat received from client at {DateTime.Now.ToString("T")}");
                }
            });

        var builder = new ServerBuilder();
        var ipAddress = new IPAddress(new byte[] { 10, 0, 1, 6 });
        builder.Configure(config =>
        {
            // TODO: Configure heartbeat and priority levels (config.Priorities.Critical = 0.1) to send updates every 0.1ms
            config.HostAddress = ipAddress;
            config.Port = 11000;
            config.PacketBufferSize = 256;
            config.MapClientDatagramTypes(factory =>
            {
                datagramFactory = factory;
                factory.RegisterDatagramType<ClientHeartBeat>(2);
            });
        });

        Console.WriteLine($"Listening on {ipAddress}");
        builder.StartListening();
    }
}