using System;
using System.Collections.Generic;
using System.Text;
using GameServ.Core;
using GameServ;

namespace GameServ.Server
{
    public class DatagramReceivedMessage : IMessage
    {
        public object GetContent()
        {
            return 0;
        }
    }
}
