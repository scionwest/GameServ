namespace GameServ
{
    public class EndPointInformation
    {
        public EndPointInformation(byte[] ipAddress, int port)
        {
            this.IPAddress = ipAddress;
            this.Port = port;
        }

        public byte[] IPAddress { get; }

        public int Port { get; }

        public override string ToString()
            => $"{string.Join(".", this.IPAddress)}:{this.Port}";
    }
}