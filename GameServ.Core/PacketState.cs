using System.Net;
using System.Net.Sockets;

namespace GameServ
{
    public class PacketState
    {
        public PacketState(Socket serverSocket, int packetBufferSize)
        {
            this.Socket = serverSocket;
            this.Buffer = new byte[packetBufferSize];
        }

        public EndPoint Destination { get; set; }

        public byte[] Buffer { get; private set; }

        public Socket Socket { get; }

        public void Reset()
        {
            for(int index = 0; index < this.Buffer.Length; index++)
            {
                this.Buffer[index] = 0;
            }
        }
    }
}