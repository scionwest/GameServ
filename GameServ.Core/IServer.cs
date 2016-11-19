using System;
using System.Net;

namespace GameServ.Core
{
    public interface IServer
    {
        event Action<ClientDatagramArgs> ClientDatagramReceived;

        event Action<IServerDatagram> ServerDatagramSent;

        int ServerPort { get; set; }

        int PacketBufferSize { get; set; }

        int ClientTimeoutPeriod { get; set; }

        bool IsEventMessagingEnabled { get; set; }

        ServerPolicy ServerPolicy { get; set; }

        bool IsRunning { get; }

        IPEndPoint GetAvailableServerEndPoint();

        void Start();

        void Shutdown();

        void SendMessage(IServerDatagram message, EndPoint destination);
    }
}
