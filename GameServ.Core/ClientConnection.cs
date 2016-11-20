using System;
using System.Net;

namespace GameServ
{
    public class ClientConnection : IClient
    {
        public ClientConnection(string appVersion, EndPoint destination, long lastTransmissionTime, string osPlatform, string osVersion)
        {
            this.AppVersion = appVersion;
            this.Destination = destination;
            this.LastTransmissionTime = lastTransmissionTime;
            this.OSPlatform = OSPlatform;
            this.OSVersion = osVersion;
        }

        public string AppVersion { get; }

        public EndPoint Destination { get; }

        public long LastTransmissionTime { get; }

        public string OSPlatform { get; }

        public string OSVersion { get; }

        public void DatagramReceived(IClientDatagram datagram)
        {
            throw new NotImplementedException();
        }

        public EndPointInformation GetTargetServer()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void SendDatagramToClient(IServerDatagram datagram)
        {
            throw new NotImplementedException();
        }
    }
}