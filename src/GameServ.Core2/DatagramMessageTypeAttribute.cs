using System;
using System.Collections.Generic;
using System.Text;

namespace GameServ
{
    public class DatagramMessageTypeAttribute : Attribute
    {
        public DatagramMessageTypeAttribute(byte messageType)
        {
            this.MessageType = messageType;
        }

        public byte MessageType { get; }
    }
}
