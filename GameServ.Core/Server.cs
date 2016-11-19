using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServ.Core
{
    public class Server : IServer
    {
        private readonly DatagramFactory datagramFactory;
        private readonly Dictionary<EndPoint, ClientConnection> connectedClients;
        private Socket serverSocket;
        private IPEndPoint serverEndPoint;

        public Server()
        {
            this.ServerPort = 11000;
            this.PacketBufferSize = 256;
            this.ClientTimeoutSeconds = (int)TimeSpan.FromMinutes(5).TotalSeconds;

            this.datagramFactory = new DatagramFactory();
            this.connectedClients = new Dictionary<EndPoint, ClientConnection>();
        }

        public event Action<ClientDatagramArgs> ClientDatagramReceived;
        public event Action<IServerDatagram> ServerDatagramSent;
        public event Action<IServer> ServerStarted;

        public int ClientTimeoutSeconds { get; set; }

        public bool IsEventMessagingEnabled { get; set; }

        public bool IsRunning { get; private set; }

        public int PacketBufferSize { get; set; }

        public ServerPolicy ServerPolicy { get; set; }

        public int ServerPort { get; set; }

        // TODO: Need to support multiple servers hidden behind load balancers. 
        // Need to return one of the available end points, not just 1.
        public IPEndPoint GetAvailableServerEndPoint()
            => this.serverEndPoint;

        public void Start()
        {
            // Setup the socket for use
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.serverEndPoint = new IPEndPoint(IPAddress.Any, this.ServerPort);

            // Bind and configure the socket so we are always given the client end point packet info.
            this.serverSocket.Bind(this.serverEndPoint);
            this.IsRunning = true;
            this.ListenForData(this.serverSocket);
            this.ServerStarted?.Invoke(this);
        }

        private void ListenForData(Socket listenOnSocket)
        {
            if (!this.IsRunning)
            {
                return;
            }

            var state = new PacketState(listenOnSocket, this.PacketBufferSize)
            {
                Destination = (EndPoint)new IPEndPoint(IPAddress.Any, this.ServerPort)
            };
            byte[] buffer = state.Buffer;
            EndPoint destination = state.Destination;

            this.serverSocket.BeginReceiveFrom(
                state.Buffer,
                0,
                this.PacketBufferSize,
                SocketFlags.None,
                ref destination,
                new AsyncCallback(this.ReceiveClientData),
                state);
        }

        private void ReceiveClientData(IAsyncResult result)
        {
            PacketState state = (PacketState)result.AsyncState;
            Socket socket = state.Socket;
            EndPoint endPoint = state.Destination;
            ClientConnection connection = null;

            int receivedData = socket.EndReceiveFrom(result, ref endPoint);
            if (receivedData == 0)
            {
                this.ListenForData(socket);
                return;
            }

            using (var reader = new BinaryReader(new MemoryStream(state.Buffer)))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                IClientHeader header = new ClientHeader();
                header.Deserialize(reader);
                if (!header.IsMessageValid())
                {
                    // We abort trying to do anything with this packet. 
                    // It is was malformed and we can't rely on it.
                    this.ListenForData(socket);
                    return;
                }

                if (header.Policy.HasFlag(DatagramPolicy.AcknoweldgementRequired) ||
                    this.ServerPolicy.HasFlag(ServerPolicy.RequireAcknowledgement))
                {
                    //var acknoweldgement = new Achoweldge { MessageCode = header.MessageType, SequenceNumber = header.SequenceNumber };
                    //this.SendMessage(acknoweldgement, endPoint);
                }

                IClientDatagram datagram = this.datagramFactory.CreateDatagramFromClientHeader(header);
                if (datagram == null)
                {
                    // TODO: Handle null. Do we just re-listen and ignore?
                }

                datagram.Deserialize(reader);

                this.ClientDatagramReceived?.Invoke(new ClientDatagramArgs(datagram, state.Buffer, connection));
            }
            this.ListenForData(socket);
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public void SendMessage(IServerDatagram message, EndPoint destination)
        {
            this.ServerDatagramSent?.Invoke(message);
        }
    }
}
