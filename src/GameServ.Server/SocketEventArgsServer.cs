using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.Reflection;
using System.Threading;
using System.IO;
using GameServ.Core;

namespace GameServ.Server
{
    internal class SocketListener : INetworkListener
    {
        private ArrayPool<byte> socketBufferPool;
        private ObjectPool<SocketAsyncEventArgs> socketEventArgsPool;
        private int count = 0;
        private SpinLock countLock = new SpinLock();

        private Socket listeningSocket;
        private IPEndPoint readEndPoint;

        private readonly ServerConfiguration configuration;
        private IEnumerable<IMiddleware> middleware;

        public SocketListener(IEnumerable<IMiddleware> middleware, ServerConfiguration config)
        {
            this.middleware = middleware;
            this.configuration = config;
            this.readEndPoint = new IPEndPoint(IPAddress.Any, 11100);
        }

        public ServerPolicy ServerPolicy { get; set; }

        public bool IsRunning { get; private set; }

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
            // Configure object pools
            this.CreateObjectPools();

            var endPoint = new IPEndPoint(this.configuration.HostAddress, this.configuration.Port);

            this.listeningSocket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            this.listeningSocket.Bind(endPoint);
            SocketAsyncEventArgs readArgs = this.socketEventArgsPool.Rent<SocketAsyncEventArgs>();
            this.listeningSocket.ReceiveFromAsync(readArgs);

            this.IsRunning = true;
            while(this.IsRunning)
            {
            }
        }

        private void ReceivedSocketEvent(object sender, SocketAsyncEventArgs e)
        {
            // Rent a new Socket event arg right away and start listening for more packets.
            SocketAsyncEventArgs eventArgs = this.socketEventArgsPool.Rent<SocketAsyncEventArgs>();

            this.listeningSocket.ReceiveFromAsync(eventArgs);

            // Get the data we want from the event args and then send it back into the pool for recycling.
            //byte[] buffer = e.Buffer;
            //ClientConnection client = (ClientConnection)e.UserToken;
            this.socketEventArgsPool.Return(e);
            Task.Run(() => MessageBroker.Default.Publish(new DatagramReceivedMessage()));

            //if (buffer.Length == 0)
            //{
            //    return;
            //}

            //using (var binaryReader = new BinaryReader(new MemoryStream(buffer)))
            //{ }
        }

        private void CreateObjectPools()
        {
            const int _poolBucketSize = 1000;
            int packetSize = this.configuration.PacketBufferSize;
            this.socketEventArgsPool = new ObjectPool<SocketAsyncEventArgs>(1000, this.ConfigureSocketEventArgs);
            this.socketBufferPool = ArrayPool<byte>.Create(256, _poolBucketSize);
        }

        private SocketAsyncEventArgs ConfigureSocketEventArgs()
        {
            byte[] buffer = this.socketBufferPool.Rent(256);
            var eventArg = new SocketAsyncEventArgs();
            eventArg = new SocketAsyncEventArgs { UserToken = new ClientConnection(this) };
            eventArg.Completed += new EventHandler<SocketAsyncEventArgs>(this.ReceivedSocketEvent);
            eventArg.SetBuffer(buffer, 0, 256);
            eventArg.RemoteEndPoint = this.readEndPoint;

            return eventArg;
        }
    }
}
