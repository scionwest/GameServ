using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServ
{
    public class ClientHeader : IClientHeader
    {
        public ClientHeader() { }
        public ClientHeader(byte channel, int clientId, bool isLastInSequence, byte messageType, DatagramPolicy policy, byte sequenceNumber)
        {
            this.Channel = channel;
            this.ClientId = ClientId;
            this.IsLastInSequence = isLastInSequence;
            this.MessageType = messageType;
            this.Policy = policy;
            this.SequenceNumber = sequenceNumber;
        }

        public byte Channel { get; private set; }

        public int ClientId { get; private set; }

        public bool IsLastInSequence { get; private set; }

        public byte MessageType { get; private set; }

        public DatagramPolicy Policy { get; private set; }

        public byte SequenceNumber { get; private set; }

        public void Deserialize(BinaryReader deserializer)
        {
            this.Channel = deserializer.ReadByte();
            this.ClientId = deserializer.ReadInt32();
            this.IsLastInSequence = deserializer.ReadBoolean();
            this.MessageType = deserializer.ReadByte();
            this.Policy = (DatagramPolicy)deserializer.ReadInt32();
            this.SequenceNumber = deserializer.ReadByte();
        }

        public bool IsMessageValid()
        {
            return true;
        }

        public void PrepareForReuse()
        {
            
        }

        public void Serialize(BinaryWriter serializer)
        {
            serializer.Write(this.Channel);
            serializer.Write(this.ClientId);
            serializer.Write(this.IsLastInSequence);
            serializer.Write(this.MessageType);
            serializer.Write((int)this.Policy);
            serializer.Write(this.SequenceNumber);
        }
    }
}
