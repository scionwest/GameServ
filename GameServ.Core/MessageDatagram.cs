﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServ.Datagrams
{
    public class MessageDatagram : IClientDatagram
    {
        public MessageDatagram(){}
        public MessageDatagram(string message) => this.Message = message;

        public string Message { get; private set; }

        public void Deserialize(BinaryReader deserializer)
        {
            this.Message = deserializer.ReadString();
        }

        public bool IsMessageValid()
        {
            return string.IsNullOrEmpty(this.Message);
        }

        public void PrepareForReuse()
        {
            this.Message = string.Empty;
        }

        public void Serialize(BinaryWriter serializer)
        {
            serializer.Write(this.Message);
        }
    }
}