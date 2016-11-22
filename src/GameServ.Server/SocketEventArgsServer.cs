using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;

namespace GameServ.Server
{
    public class SocketEventArgsServer : IServer
    {
        private ArrayPool<byte> socketBufferPool;
        private IEnumerable<IMiddleware> middleware;

        public SocketEventArgsServer()
        {

        }

        public int ClientTimeoutSeconds { get; set; }

        public bool IsAuthenticationRequired { get; set; }

        public bool IsEventMessagingEnabled { get; set; }

        public bool IsRunning { get; }

        public int PacketBufferSize { get; set; }

        public ServerPolicy ServerPolicy { get; set; }

        public int ServerPort { get; set; }

        public event Action<ClientDatagramArgs> ClientDatagramReceived;
        public event Action<IServerDatagram> ServerDatagramSent;

        internal void UseMiddleware(IEnumerable<IMiddleware> middleware)
        {
            this.middleware = middleware;
        }

        public IPEndPoint GetAvailableServerEndPoint()
        {
            throw new NotImplementedException();
        }

        public void PurgeStaleConnections()
        {
            throw new NotImplementedException();
        }

        public void SendMessage(ClientConnection connection, IServerDatagram message)
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
