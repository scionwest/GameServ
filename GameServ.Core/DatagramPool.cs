using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServ.Core
{
    public class DatagramPool
    {
        private readonly int maxPoolSize;
        private readonly Dictionary<Type, Stack<IDatagram>> poolCache;

        public DatagramPool(int poolSize)
        {
            this.maxPoolSize = poolSize;
            this.poolCache = new Dictionary<Type, Stack<IDatagram>>();
        }

        public TDatagram TakeOne<TDatagram>() where TDatagram : IDatagram
        {
            Type datagramType = typeof(TDatagram);
            Stack<IDatagram> cachedCollection = this.GetCacheForDatagram<TDatagram>();
            if (cachedCollection.Count == 0)
            {
                // New instances don't need to be prepared for re-use, so we just return it.
                return (TDatagram)Activator.CreateInstance(datagramType);
            }

            // Have the datagram clean itself up before being re-used.
            // We do it when being asked for, vs when released, since it might not always be re-used.
            // Prevents needlessly cleaning itself up when not needed.
            IDatagram datagram = cachedCollection.Pop();
            datagram.PrepareForReuse();
            return (TDatagram)datagram;
        }

        public void Release<TDatagram>(TDatagram datagram) where TDatagram : IDatagram
        {
            Stack<IDatagram> cachedCollection = this.GetCacheForDatagram<TDatagram>();

            // We don't need to re-add this datagram to our pool if we've hit our max pool size.
            // This would help in stopping the server from running out of memory if someone got cute
            // and figured out the protocol and spammed the server with messages.
            if (cachedCollection.Count >= this.maxPoolSize)
            {
                return;
            }

            cachedCollection.Push(datagram);
        }

        private Stack<IDatagram> GetCacheForDatagram<TDatagram>() where TDatagram : IDatagram
        {
            Type datagramType = typeof(TDatagram);
            if (!this.poolCache.TryGetValue(datagramType, out Stack<IDatagram> cachedCollection))
            {
                cachedCollection = new Stack<IDatagram>();
                this.poolCache.Add(datagramType, cachedCollection);
            }

            return cachedCollection;
        }
    }
}
