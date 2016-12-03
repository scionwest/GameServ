using System.Net;

namespace GameServ
{
    public class ConnectionState
    {
        private INetworkListener server;
        private IClientDatagram lastReveivedDatagram;

        public ConnectionState(INetworkListener server) => this.server = server;

        public long LastTransmissionTime { get; private set; }

        public byte AppVersion { get; private set; }

        public byte OSPlatform { get; private set; }

        public string OSVersion { get; private set; }

        public void DatagramReceived(IClientDatagram datagram)
        {
            this.LastTransmissionTime = datagram.Header.TimeStamp;
            this.AppVersion = datagram.Header.AppVersion;
            this.OSPlatform = datagram.Header.OSPlatform;
            this.OSVersion = datagram.Header.OSVersion;
            this.lastReveivedDatagram = datagram;
        }
    }
}