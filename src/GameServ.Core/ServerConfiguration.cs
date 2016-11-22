using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GameServ
{
    public class ServerConfiguration
    {
        internal DatagramFactory DatagramFactory { get; } = new DatagramFactory();

        public int Port { get; set; } = 11000;

        public IPAddress HostAddress { get; set; } = IPAddress.Loopback;

        public int PacketBufferSize { get; set; } = 256;

        public ServerPolicy Policy { get; set; } = ServerPolicy.None;

        public ServerConfiguration MapClientDatagramTypes(Action<DatagramFactory> factory)
        {
            factory(this.DatagramFactory);
            return this;
        }
    }
}
