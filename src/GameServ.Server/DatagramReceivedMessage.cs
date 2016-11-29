using System;
using System.Collections.Generic;
using System.Text;
using GameServ.Core;
using GameServ;
using System.IO;

namespace GameServ.Server
{
    public class DatagramReceivedMessage : IMessage<BinaryReader>
    {
        private BinaryReader reader;

        public DatagramReceivedMessage(BinaryReader binaryReader, ClientHeader clientHeader)
        {
            this.reader = binaryReader;
            this.Header = clientHeader;
        }

        public BinaryReader Content => this.reader;

        public ClientHeader Header { get; }

        public object GetContent() 
            => this.reader;
    }
}
