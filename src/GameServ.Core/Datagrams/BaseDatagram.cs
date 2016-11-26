using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameServ.Core.Datagrams
{
    public class BaseDatagram : IClientDatagram
    {
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

        protected virtual void SetHeader(IDatagramHeader header)
        {
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
