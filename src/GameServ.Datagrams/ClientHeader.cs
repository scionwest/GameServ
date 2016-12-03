using System.IO;

namespace GameServ.Datagrams
{
    public class ClientHeader : ServerHeader, IClientDatagramHeader
    {
        public ClientHeader() { }

        public ClientHeader(byte channel, int clientId, bool isLastInSequence, byte messageType, DatagramPolicy policy, byte sequenceNumber)
            : base(channel, clientId, isLastInSequence, messageType, policy, sequenceNumber)
        {

            this.OSPlatform = 1;
            this.OSVersion = "Win10";
            this.AppVersion = 1;
        }

        public byte OSPlatform { get; set; }

        public string OSVersion { get; set; }

        public byte AppVersion { get; set; }

        protected override void OnDeserialization(BinaryReader deserializer)
        {
            this.OSPlatform = deserializer.ReadByte();
            this.OSVersion = deserializer.ReadString();
            this.AppVersion = deserializer.ReadByte();
        }

        protected override void OnSerialization(BinaryWriter serializer)
        {
            serializer.Write(this.OSPlatform);
            serializer.Write(this.OSVersion);
            serializer.Write(this.AppVersion);
        }

        protected override bool OnValidationCheck(bool isCurrentlyValid)
            => isCurrentlyValid
            && this.Channel > 0
            && this.ClientId > 0
            && OSPlatform > 0
            && AppVersion > 0
            && !string.IsNullOrEmpty(OSVersion);

        protected override void OnReset()
        {
            this.OSPlatform = 0;
            this.OSVersion = string.Empty;
            this.AppVersion = 0;
        }
    }
}
