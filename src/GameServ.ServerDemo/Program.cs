﻿using GameServ;
using GameServ.Core;
using GameServ.Core.Datagrams.Client;
using GameServ.Core.NetworkReplication;
using GameServ.Server;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    public class DatagramReader
    {
        private byte[] buffer;

        public DatagramReader(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public int Length => this.buffer.Length;

        public int Position { get; private set; }

        public int ReadInt()
        {
            return -1;
        }
    }

    static void Main(string[] args)
    {
        MessageBroker.Default.Subscribe<DatagramReceivedMessage>(
            (msg, sub) => Console.WriteLine(msg.Content.ReadString()));
        var builder = new ServerBuilder();
        var ipAddress = new IPAddress(new byte[] { 10, 0, 1, 6 });
        builder.Configure(config =>
        {
            config.HostAddress = ipAddress;
            config.Port = 11000;
            config.PacketBufferSize = 256;
        });

        Console.WriteLine($"Listening on {ipAddress}");
        builder.StartListening();
    }
}