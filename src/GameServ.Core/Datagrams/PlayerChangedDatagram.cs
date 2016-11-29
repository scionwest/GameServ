using System.IO;

namespace GameServ.Core.Datagrams
{
    public class PlayerChangedDatagram : IClientDatagram
    {
        public PlayerChangedDatagram()
        {
            this.Header = new ClientHeader(
                1, 2, false, 10, DatagramPolicy.AcknoweldgementRequired, 0);
        }

        public IClientDatagramHeader Header { get; set; }

        public string Username { get; private set; }
        public int Age { get; private set; }
        public bool CanDrive { get; private set; }

        public void Deserialize(BinaryReader deserializer)
        {
            this.Username = deserializer.ReadString();
            this.Age = deserializer.ReadInt32();
            this.CanDrive = deserializer.ReadBoolean();
        }

        public bool IsMessageValid()
        {
            return !string.IsNullOrEmpty(this.Username);
        }

        public void PrepareForReuse()
        {
            this.Username = string.Empty;
        }

        public void Serialize(BinaryWriter serializer)
        {
            serializer.Write(this.Username);
            serializer.Write(this.Age);
            serializer.Write(this.CanDrive);
        }
    }
}
