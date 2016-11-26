using System;
using System.Collections.Generic;
using System.Text;

namespace GameServ.Core
{
    public class NotificationArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationArgs"/> class.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="messageType">Type of the message.</param>
        public NotificationArgs(ISubscription subscription, Type messageType)
        {
            this.Subscription = subscription;
            this.MessageType = messageType;
        }

        /// <summary>
        /// Gets the subscription, allowing for delegates to unsubscribe from future publications if needed.
        /// </summary>
        public ISubscription Subscription { get; }

        /// <summary>
        /// Gets the message payload.
        /// </summary>
        public Type MessageType { get; }
    }
}
