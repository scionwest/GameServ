using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServ.Datagrams
{
    public class MessageDatagram : IClientDatagram
    {
        public MessageDatagram(string message) => this.Message = message;

        public IClientDatagramHeader Header
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string Message { get; private set; }

        public long TimeStamp
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Deserialize(BinaryReader deserializer)
        {
            this.Message = deserializer.ReadString();
        }

        public bool IsMessageValid()
        {
            return string.IsNullOrEmpty(this.Message);
        }

        public void PrepareForReuse()
        {
            this.Message = string.Empty;
        }

        public void Serialize(BinaryWriter serializer)
        {
            serializer.Write(this.Message);
        }
    }
}
