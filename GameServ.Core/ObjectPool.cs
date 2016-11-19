using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServ.Core
{
    public class ObjectPool<TObject> where TObject : IPoolable
    {
        private readonly int maxPoolSize;
        private readonly Dictionary<Type, Stack<TObject>> poolCache;

        public ObjectPool(int poolSize)
        {
            this.maxPoolSize = poolSize;
            this.poolCache = new Dictionary<Type, Stack<TObject>>();
        }

        public T TakeOne<T>() where T : TObject
            => (T)this.TakeOne(typeof(T));

        public TObject TakeOne(Type datagramType)
        {
            Stack<TObject> cachedCollection = this.GetCacheForDatagram(datagramType);
            if (cachedCollection.Count == 0)
            {
                // New instances don't need to be prepared for re-use, so we just return it.
                return (TObject)Activator.CreateInstance(datagramType);
            }

            // Have the datagram clean itself up before being re-used.
            // We do it when being asked for, vs when released, since it might not always be re-used.
            // Prevents needlessly cleaning itself up when not needed.
            TObject datagram = cachedCollection.Pop();
            datagram.PrepareForReuse();
            return datagram;

        }

        public void Release(TObject datagram)
        {
            Stack<TObject> cachedCollection = this.GetCacheForDatagram(datagram.GetType());

            // We don't need to re-add this datagram to our pool if we've hit our max pool size.
            // This would help in stopping the server from running out of memory if someone got cute
            // and figured out the protocol and spammed the server with messages.
            if (cachedCollection.Count >= this.maxPoolSize)
            {
                return;
            }

            cachedCollection.Push(datagram);
        }

        private Stack<TObject> GetCacheForDatagram(Type datagramType)
        {
            if (!this.poolCache.TryGetValue(datagramType, out Stack<TObject> cachedCollection))
            {
                cachedCollection = new Stack<TObject>();
                this.poolCache.Add(datagramType, cachedCollection);
            }

            return cachedCollection;
        }
    }
}
