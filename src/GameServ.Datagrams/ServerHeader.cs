using System.IO;

namespace GameServ.Datagrams
{
    public class ServerHeader : IDatagramHeader
    {
        public ServerHeader() { }
        public ServerHeader(byte channel, int clientId, bool isLastInSequence, byte messageType, DatagramPolicy policy, byte sequenceNumber)
        {
            this.Channel = channel;
            this.ClientId = clientId;
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

        public long TimeStamp { get; private set; }

        public void Deserialize(BinaryReader deserializer)
        {
            this.Channel = deserializer.ReadByte();
            this.ClientId = deserializer.ReadInt32();
            this.IsLastInSequence = deserializer.ReadBoolean();
            this.MessageType = deserializer.ReadByte();
            this.Policy = (DatagramPolicy)deserializer.ReadInt32();
            this.SequenceNumber = deserializer.ReadByte();
            this.OnDeserialization(deserializer);
        }

        public void Serialize(BinaryWriter serializer)
        {
            serializer.Write(this.Channel);
            serializer.Write(this.ClientId);
            serializer.Write(this.IsLastInSequence);
            serializer.Write(this.MessageType);
            serializer.Write((int)this.Policy);
            serializer.Write(this.SequenceNumber);
            this.OnSerialization(serializer);
        }

        public void Reset()
        {
            this.OnReset();
        }

        public bool IsValid()
        {
            return this.OnValidationCheck(true);
        }

        protected virtual void OnDeserialization(BinaryReader deserializer)
        {
        }

        protected virtual void OnSerialization(BinaryWriter serializer)
        {
        }

        protected virtual void OnReset()
        {
        }

        protected virtual bool OnValidationCheck(bool isCurrentlyValid)
        {
            return isCurrentlyValid;
        }
    }
}
