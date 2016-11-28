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

        public DatagramReceivedMessage(BinaryReader binaryReader) 
            =>this.reader = binaryReader;

        public BinaryReader Content => this.reader;

        public object GetContent() 
            => this.reader;
    }
}
