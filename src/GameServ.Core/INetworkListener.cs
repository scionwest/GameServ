using System;
using System.Net;

namespace GameServ
{
    public interface INetworkListener
    {
        ServerPolicy ServerPolicy { get; set; }

        bool IsRunning { get; }

        void Start();

        void Shutdown();

        void SendMessage(ConnectionState connection, IServerDatagram message);
    }
}
