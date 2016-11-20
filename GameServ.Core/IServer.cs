using System;
using System.Net;

namespace GameServ
{
    public interface IServer
    {
        event Action<ClientDatagramArgs> ClientDatagramReceived;

        event Action<IServerDatagram> ServerDatagramSent;

        int ServerPort { get; set; }

        int PacketBufferSize { get; set; }

        int ClientTimeoutSeconds { get; set; }

        bool IsEventMessagingEnabled { get; set; }

        ServerPolicy ServerPolicy { get; set; }

        bool IsRunning { get; }

        bool IsAuthenticationRequired { get; set; }

        IPEndPoint GetAvailableServerEndPoint();

        void Start();

        void Shutdown();

        void SendMessage(ClientConnection connection, IServerDatagram message);

        void PurgeStaleConnections();
    }
}
