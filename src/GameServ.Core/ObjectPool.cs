﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GameServ
{
    public class ObjectPool<TObject>
    {
        private int maxPoolSize;
        private Dictionary<Type, Stack<TObject>> poolCache;
        private Func<TObject> factory;

        public ObjectPool(int poolSize)
        {
            this.maxPoolSize = poolSize;
            this.poolCache = new Dictionary<Type, Stack<TObject>>();
        }

        public ObjectPool(int poolSize, Func<TObject> factory) : this(poolSize)
        {
            this.factory = factory;
        }

        public T Rent<T>() where T : TObject
            => (T)this.Rent(typeof(T));

        public TObject Rent(Type type)
        {
            //bool lockTaken = false;
            Stack<TObject> cachedCollection;

            TObject instance = default(TObject);
            Monitor.Enter(this.poolCache);
            if (!this.poolCache.TryGetValue(type, out cachedCollection))
            {
                cachedCollection = new Stack<TObject>();
                this.poolCache.Add(type, cachedCollection);
            }

            if (cachedCollection.Count > 0)
            {
                instance = cachedCollection.Pop();
            }
            Monitor.Exit(this.poolCache);

            if (instance != null)
                return instance;

            // New instances don't need to be prepared for re-use, so we just return it.
            if (this.factory == null)
            {
                return (TObject)Activator.CreateInstance(type);
            }
            else
            {
                return this.factory();
            }
        }

        public void Return(TObject instanceObject)
        {
            Stack<TObject> cachedCollection = null;
            Type type = typeof(TObject);

            Monitor.Enter(poolCache);
            if (!this.poolCache.TryGetValue(type, out cachedCollection))
            {
                cachedCollection = new Stack<TObject>();
                this.poolCache.Add(type, cachedCollection);
            }

            if (cachedCollection.Count >= this.maxPoolSize)
            {
                return;
            }

            // TODO: Convert Stack into an array.
            // We'll track the current index by decrementing the index in Rent, and incrementing in Return
            // Upon Renting, we'll set pool[index] to null, then reduce index by 1. This lets us manage an
            // array and have zero lookup costs.
            cachedCollection.Push(instanceObject);
            Monitor.Exit(poolCache);
        }
    }
}
