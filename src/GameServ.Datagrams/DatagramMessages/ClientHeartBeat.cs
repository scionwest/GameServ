using System.IO;

namespace GameServ.Datagrams.DatagramMessages
{
    public class ClientHeartBeat : ClientDatagram
    {
        public override void Deserialize(BinaryReader deserializer)
        {
        }

        public override bool IsValid() => true;

        public override void Reset()
        {
        }

        public override void Serialize(BinaryWriter serializer)
        {
        }
    }
}
