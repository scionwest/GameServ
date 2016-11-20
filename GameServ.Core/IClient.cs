using System.Net;

namespace GameServ
{
    public interface IClient
    {
        EndPoint Destination { get; }

        long LastTransmissionTime { get; }

        string AppVersion { get; }

        string OSPlatform { get; }

        string OSVersion { get; }

        void Initialize();

        void SendDatagramToClient(IServerDatagram datagram);

        void DatagramReceived(IClientDatagram datagram);

        EndPointInformation GetTargetServer();
    }
}