using System.Net;
using System.Net.Sockets;

namespace GameServ.Core
{
    internal class PacketState
    {
        private int packetBufferSize;
        private Socket serverSocket;

        public PacketState(Socket serverSocket, int packetBufferSize)
        {
            this.serverSocket = serverSocket;
            this.packetBufferSize = packetBufferSize;
        }

        public EndPoint Destination { get; set; }

        public byte[] Buffer { get; private set; }

        public Socket Socket { get; set; }
    }
}