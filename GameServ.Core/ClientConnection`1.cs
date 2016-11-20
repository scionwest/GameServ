using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameServ
{
    public class ClientConnection<TOwner> : ClientConnection, IClient<TOwner>
    {
        public ClientConnection(string appVersion, EndPoint destination, long lastTransmissionTime, string osPlatform, string osVersion, TOwner owner) 
            : base(appVersion, destination, lastTransmissionTime, osPlatform, osVersion)
        {
            this.Owner = owner;
        }

        public TOwner Owner { get; }
    }
}
