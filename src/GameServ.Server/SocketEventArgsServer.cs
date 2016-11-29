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
using System.Diagnostics;

namespace GameServ.Server
{
    internal class SocketListener : INetworkListener
    {
        private ArrayPool<byte> socketBufferPool;
        private ObjectPool<SocketAsyncEventArgs> socketEventArgsPool;
        private Dictionary<SocketAsyncEventArgs, ConnectionState> clientState;

        private Socket listeningSocket;
        private IPEndPoint readEndPoint;

        private readonly ServerConfiguration configuration;

        public SocketListener(ServerConfiguration config)
        {
            this.configuration = config;
            this.readEndPoint = new IPEndPoint(IPAddress.Any, 11100);
        }

        public ServerPolicy ServerPolicy { get; set; }

        public bool IsRunning { get; private set; }

        public void SendMessage(ConnectionState connection, IServerDatagram message)
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
            // We have to grab a reference to the buffer array. Once the rental is returned we can't
            // trust it's data, it might be in use by another SocketAsyncEventArgs at any point in time.
            byte[] buffer = e.Buffer;
            var client = (ConnectionState)e.UserToken;
            this.socketEventArgsPool.Return(e);

            if (buffer.Length == 0)
            {
                return;
            }

            using (var binaryReader = new BinaryReader(new MemoryStream(buffer)))
            {
                var header = new ClientHeader();
                header.Deserialize(binaryReader);
                MessageBroker.Default.Publish(new DatagramReceivedMessage(binaryReader, header));
            }
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
            eventArg = new SocketAsyncEventArgs { UserToken = new ConnectionState(this) };
            eventArg.Completed += new EventHandler<SocketAsyncEventArgs>(this.ReceivedSocketEvent);
            eventArg.SetBuffer(buffer, 0, 256);
            eventArg.RemoteEndPoint = this.readEndPoint;

            return eventArg;
        }
    }
}
