using System.IO;

namespace GameServ.Datagrams
{
    public abstract class ClientDatagram : IClientDatagram
    {
        public IClientDatagramHeader Header { get; set; }

        public abstract void Deserialize(BinaryReader deserializer);

        public abstract void Serialize(BinaryWriter serializer);

        public abstract bool IsValid();

        public abstract void Reset();
    }
}
