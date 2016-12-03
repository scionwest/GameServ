using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GameServ.Core
{
    public class MessageBroker : IMessageBroker
    {
        /// <summary>
        /// Collection of subscribed listeners
        /// </summary>
        private Dictionary<Type, List<ISubscription>> listeners;
        private ArrayPool<ISubscription> publishingCollectionPool;
        private SpinLock listenerLock;
        private static IMessageBroker singleton;

        static MessageBroker() => singleton = new MessageBroker();

        public MessageBroker()
        {
            this.listeners = new Dictionary<Type, List<ISubscription>>();
            this.listenerLock = new SpinLock(Debugger.IsAttached);
            this.publishingCollectionPool = ArrayPool<ISubscription>.Create();
        }

        public static IMessageBroker Default => MessageBroker.singleton;

        /// <summary>
        /// Subscribe publications for the message type specified.
        /// @code
        /// private ISubscription whisperSubscription;
        /// 
        /// public void Initialize()
        /// {
        ///     this.whisperSubscription = notificationManager.Subscribe<WhisperMessage>((msg, sub) => DoStuff);
        /// }
        /// @endcode
        /// </summary>
        /// <typeparam name="TMessageType">A concrete implementation of IMessage</typeparam>
        /// <returns></returns>
        public ISubscription Subscribe<TMessageType>(Action<TMessageType, ISubscription> callback, Func<TMessageType, bool> condition = null) where TMessageType : class, IMessage
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback must not be null when subscribing");
            }

            Type messageType = typeof(TMessageType);

            // Add our notification to our listener collection so we can publish to it later, then return it.
            // TODO: Move instancing the Notification in to a Factory.
            var handler = new Notification<TMessageType>();
            handler.Register(callback, condition);
            handler.Unsubscribing += this.Unsubscribe;

            // Lock the collection while we mess with it.
            // In most cases this will be quick - a Key lookup will be done (twice) and a new item added to the collection.
            // Should not impact anything negatively

            Monitor.Enter(listeners);
            // Create our key if it doesn't exist along with an empty collection as the value.
            if (!listeners.TryGetValue(messageType, out var subscriptions))
            {
                subscriptions = new List<ISubscription>();
                subscriptions.Add(handler);
                listeners.Add(messageType, subscriptions);
            }
            else
            {
                subscriptions.Add(handler);
            }

            Monitor.Exit(listeners);
            return handler;
        }

        /// <summary>
        /// Publishes the specified message to all subscribers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        public void Publish<T>(T message) where T : class, IMessage
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "You can not publish a null message.");
            }

            Type messageType = typeof(T);
            if (!this.listeners.TryGetValue(messageType, out var subscribers))
            {
                return;
            }

            // Create a local reference of the collection to protect us against the collection
            // adding a new subscriber while we're enumerating.
            // It is cheaper for us to lock the existing collection and transfer references to a 
            // collection provided by the pool, then iterate publishes. 
            // If we lock while publishing, the overhead could be substantial depending on what's being published.
            // We don't want to create a new collection, such as .ToArray(), as that would create to many allocations
            // over the life-time of the broker.
            Monitor.Enter(listeners);
            var listenersToPublishTo = this.publishingCollectionPool.Rent(subscribers.Count);
            for (int index = 0; index < subscribers.Count; index++)
            {
                listenersToPublishTo[index] = subscribers[index];
            }
            Monitor.Exit(listeners);

            foreach (INotification<T> handler in listenersToPublishTo)
            {
                handler?.ProcessMessage(message);
            }
        }

        /// <summary>
        /// Unsubscribes the specified handler by removing their handler from our collection.
        /// </summary>
        /// <typeparam name="T">The message Type you want to unsubscribe from</typeparam>
        /// <param name="subscription">The subscription to unsubscribe.</param>
        void Unsubscribe(NotificationArgs args)
        {
            // If the key doesn't exist or has an empty collection we just return.
            // We will leave the key in there for future subscriptions to use.
            if (!this.listeners.TryGetValue(args.MessageType, out var subscribers))
            {
                return;
            }

            if (subscribers.Count == 0)
            {
                return;
            }

            // Remove the subscription from the collection associated with the key.
            Monitor.Enter(listeners);
            subscribers.Remove(args.Subscription);
            Monitor.Exit(listeners);

            args.Subscription.Unsubscribing -= this.Unsubscribe;
        }
    }
}