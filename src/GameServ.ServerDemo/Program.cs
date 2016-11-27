using GameServ;
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
        //BenchmarkTypeKeyLookup();
        var builder = new ServerBuilder();
        var ipAddress = new IPAddress(new byte[] { 10, 0, 1, 6 });
        builder.Configure(config =>
        {
            config.HostAddress = ipAddress;
            config.Port = 11000;
            config.PacketBufferSize = 256;
            //config.MapClientDatagramTypes(factory =>
            //{
            //    factory.RegisterDatagramType<HandshakeRequest>(1);
            //}

            //config.ReplicateObject<Player>()
            //    .WithDatagram<LocationCHangedDatagram>(player => player.Location)
            //        .Bidirectional(withFrequency: config.Frequency)
            //    .WithDatagram<PhysicsChangedDatagram>(player => player.Location && player.Rotation)
            //        .BidDirectional()
            //    .WithRPC(player => player.Disconnect())
            //    .WithDatagram<LootboxCountChangedDatagram>(player => player.LootboxCount)
            //        .OneWayFromServer();
        });

        var watch = new Stopwatch();
        watch.Start();
        Console.WriteLine($"Listening on {ipAddress}");
        var msgLock = new SpinLock();
        Task.Run(() =>
        {
            int count = 0;

            MessageBroker.Default.Subscribe<DatagramReceivedMessage>((msg, sub) =>
            {
                bool lockTaken = false;
                int currentCount = count;
                try
                {
                    msgLock.Enter(ref lockTaken);
                    watch.Stop();
                    count = 0;
                    watch.Reset();
                    watch.Start();
                }
                finally
                {
                    if (lockTaken)
                    {
                        msgLock.Exit(false);
                    }
                }

                Debug.Write($"\rRequests per second: {currentCount}");
            },
            (msg) =>
            {
                count++;
                return watch.Elapsed.Seconds >= 1;
            });
        });
        builder.StartListening();
    }


    private static void BenchmarkTypeKeyLookup()
    {
        var cache = new Dictionary<Type, IEnumerable<Type>>();
        var deps = DependencyContext.Default;
        foreach (var compilationLibrary in deps.CompileLibraries)
        {
            try
            {
                Assembly assembly = Assembly.Load(new AssemblyName(compilationLibrary.Name));
                foreach (Type type in assembly.GetTypes())
                {
                    if (cache.ContainsKey(type))
                    {
                        continue;
                    }

                    cache.Add(type, assembly.GetTypes());
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        var watch = new Stopwatch();
        var time = new List<double>();
        // warm up
        foreach (KeyValuePair<Type, IEnumerable<Type>> kvp in cache)
        {
            watch.Start();
            IEnumerable<Type> value = null;
            if (cache.ContainsKey(kvp.Key))
            {
                value = cache[kvp.Key];
            }
            watch.Stop();
            time.Add(watch.Elapsed.Ticks);
            watch.Reset();
        }

        //test key lookup
        foreach (KeyValuePair<Type, IEnumerable<Type>> kvp in cache)
        {
            watch.Start();
            IEnumerable<Type> value = null;
            if (cache.ContainsKey(kvp.Key))
            {
                value = cache[kvp.Key];
            }
            watch.Stop();
            time.Add(watch.Elapsed.Ticks);
            watch.Reset();
        }
        Console.WriteLine("Key check Benchmark completed");
        Console.WriteLine($"Average time: {time.Average()} ticks");
        Console.WriteLine($"Total time: {time.Sum()} ticks");

        // test tryget
        time.Clear();
        foreach (KeyValuePair<Type, IEnumerable<Type>> kvp in cache)
        {
            watch.Start();
            if (cache.TryGetValue(kvp.Key, out var value))
            {
            }
            watch.Stop();
            time.Add(watch.Elapsed.Ticks);
            watch.Reset();
        }
        Console.WriteLine("TryGet Benchmark completed");
        Console.WriteLine($"Average time: {time.Average()} ticks");
        Console.WriteLine($"Total time: {time.Sum()} ticks");

        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }
}