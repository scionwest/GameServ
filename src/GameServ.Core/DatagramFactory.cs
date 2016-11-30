using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameServ
{
    public class DatagramAlreadyRegisteredException : Exception { public DatagramAlreadyRegisteredException(string message) : base(message) { } }

    public class DatagramFactory
    {
        private readonly Type clientDatagramType;
        private readonly Dictionary<byte, Type> clientDatagrams;
        private Dictionary<Type, ObjectPool<IClientDatagram>> clientDatagramPool;

        public DatagramFactory()
        {
            this.clientDatagramType = typeof(IClientDatagram);
            this.clientDatagrams = new Dictionary<byte, Type>();
            this.clientDatagramPool = new Dictionary<Type, ObjectPool<IClientDatagram>>();
        }

        public void RegisterDatagramType<TDatagram>(byte messageType) where TDatagram : IClientDatagram
        {
            this.RegisterDatagramType(typeof(TDatagram), messageType);
        }

        public void RegisterDatagramType(Type datagramType, byte messageType)
        {
            if (datagramType == typeof(IClientDatagram))
            {
                throw new NotSupportedException("The datagram type provided does not implement IClientDatagram");
            }

            if (this.clientDatagrams.TryGetValue(messageType, out var previouslyRegisteredType))
            {
                if (previouslyRegisteredType == datagramType)
                {
                    throw new DatagramAlreadyRegisteredException($"{datagramType.Name} has already been registered.");
                }
                else
                {
                    throw new DatagramAlreadyRegisteredException($"{messageType} has already been registered to a different Datagram. Message types must be unique.");
                }
            }

            this.clientDatagrams.Add(messageType, datagramType);
            this.clientDatagramPool.Add(datagramType, new ObjectPool<IClientDatagram>(20));
        }

        public IClientDatagram CreateDatagramFromClientHeader(IClientDatagramHeader header)
        {
            if (!this.clientDatagrams.TryGetValue(header.MessageType, out Type datagramType))
            {
                return null;
            }

            IClientDatagram datagram = this.clientDatagramPool[datagramType].Rent(datagramType);
            datagram.Header = header;
            return datagram;
        }
    }
}
