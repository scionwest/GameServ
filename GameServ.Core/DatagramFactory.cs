using GameServ.Datagrams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServ
{
    public class DatagramFactory
    {
        private readonly Dictionary<byte, Type> clientDatagrams = new Dictionary<byte, Type>();
        private readonly ObjectPool<IClientDatagram> datagramPool = new ObjectPool<IClientDatagram>(10000);

        public DatagramFactory()
        {
            clientDatagrams.Add(1, typeof(MessageDatagram));
        }

        public IClientDatagram CreateDatagramFromClientHeader(IClientDatagramHeader header)
        {
            if (!this.clientDatagrams.TryGetValue(header.MessageType, out Type datagramType))
            {
                return null;
            }

            IClientDatagram datagram = this.datagramPool.TakeOne(datagramType);
            datagram.Header = header;
            return datagram;
        }
    }
}
