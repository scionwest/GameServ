using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameServ.Core.Datagrams.Client
{
    public class Player
    {
        //public NetworkComponent Network { get; set; }

        public Player()
        {
            //Network.DatagramReceived += this.ReceiveDatagram;
        }

        //public string ReceiveDatagram(IClientDatagram gram)
        //{
        //    switch (gram.Header.MessageType)
        //    {
        //        //case 1: return this.UpdateLocation((LocationDatagram)gram);
        //        //case 1: return this.UpdateLocation((LocationDatagram)gram);
        //        //case 1: return this.UpdateLocation((LocationDatagram)gram);
        //        //case 1: return this.UpdateLocation((LocationDatagram)gram);
        //        //case 1: return this.UpdateLocation((LocationDatagram)gram);
        //        //case 1: return this.UpdateLocation((LocationDatagram)gram);
        //        //case 1: return this.UpdateLocation((LocationDatagram)gram);
        //        //default: return string.Empty;
        //    }
        //}

        public void Deserialize(BinaryReader deserializer)
        {
        }

        public void Serialize(BinaryWriter serializer)
        {
        }

        public bool IsMessageValid()
        {
            throw new NotImplementedException();
        }

        public void PrepareForReuse()
        {
            throw new NotImplementedException();
        }
    }

    public class HandshakeRequest : IClientDatagram
    {
        public IClientDatagramHeader Header { get; set; }

        public void Deserialize(BinaryReader deserializer)
        {
            throw new NotImplementedException();
        }

        public void Serialize(BinaryWriter serializer)
        {

        }

        public bool IsMessageValid()
        {
            throw new NotImplementedException();
        }

        public void PrepareForReuse()
        {
            throw new NotImplementedException();
        }
    }
}
