using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServ.Core
{
    public class ClientHeader : IClientHeader
    {
        public byte Channel
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int ClientId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsLastInSequence
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public byte MessageType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DatagramPolicy Policy
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public byte SequenceNumber
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Deserialize(BinaryReader deserializer)
        {
            throw new NotImplementedException();
        }

        public bool IsMessageValid()
        {
            throw new NotImplementedException();
        }

        public void PrepareForReuse()
        {
            throw new NotImplementedException();
        }

        public void Serialize(BinaryWriter serializer)
        {
            throw new NotImplementedException();
        }
    }
}
