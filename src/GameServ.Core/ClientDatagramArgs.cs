namespace GameServ
{
    public class ClientDatagramArgs
    {
        public ClientDatagramArgs(IClientDatagram datagram, byte[] rawMessage, ConnectionState client)
        {
            this.Datagram = datagram;
            this.RawMessage = rawMessage;
            this.Target = client;
        }

        public IClientDatagram Datagram { get; }

        public byte[] RawMessage { get; }

        public ConnectionState Target { get; }
    }
}
