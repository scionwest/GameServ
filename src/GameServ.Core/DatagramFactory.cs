using GameServ.Datagrams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServ
{
    public class DatagramAlreadyRegisteredException : Exception { public DatagramAlreadyRegisteredException(string message) : base(message) { } }

    public class DatagramFactory
    {
        private readonly Type clientDatagramType = typeof(IClientDatagram);
        private readonly Dictionary<byte, Type> clientDatagrams = new Dictionary<byte, Type>();
        private readonly ObjectPool<IClientDatagram> datagramPool = new ObjectPool<IClientDatagram>(10000);

        public DatagramFactory()
        {
            clientDatagrams.Add(1, typeof(MessageDatagram));
        }

        public void RegisterDatagramType<TDatagram>(byte messageType) where TDatagram : IClientDatagram
        {
            this.RegisterDatagramType(typeof(TDatagram), messageType);
        }

        public void RegisterDatagramType(params Type[] datagramType)
        {
            foreach (Type type in datagramType)
            {
                DatagramMessageTypeAttribute attribute = type.GetTypeInfo().GetCustomAttribute<DatagramMessageTypeAttribute>();
                this.RegisterDatagramType(type, attribute.MessageType);
            }
        }

        public void RegisterDatagramType(Type datagramType, byte messageType)
        {
            if (datagramType.GetTypeInfo().ImplementedInterfaces.Contains(clientDatagramType))
            {
                throw new NotSupportedException("The datagram type provided does not implement IClientDatagram");
            }

            if (this.clientDatagrams.ContainsKey(messageType))
            {
                if (this.clientDatagrams[messageType] == datagramType)
                {
                    throw new DatagramAlreadyRegisteredException($"{datagramType.Name} has already been registered.");
                }
                else
                {
                    throw new DatagramAlreadyRegisteredException($"{messageType} has already been registered to a different Datagram. Message types must be unique.");
                }
            }

            this.clientDatagrams.Add(messageType, datagramType);
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
