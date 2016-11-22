using GameServ.Datagrams;
using System;
using Xunit;

namespace GameServ.Server.Tests
{
    public class FakeMiddleware : IMiddleware
    {
        private readonly bool result;

        public FakeMiddleware(bool result)
        {
            this.result = result;
        }

        public bool EvaluateClientDatagram(byte[] buffer, IClientDatagram clientDatagram)
        {
            return result;
        }
    }

    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var builder = new ServerBuilder();
            builder.Configure(configuration =>
            {
                configuration.Policy = ServerPolicy.RequireAcknowledgement;
                configuration.Port = 11000;
                configuration.PacketBufferSize = 256;
                configuration.MapClientDatagramTypes(this.MapDatagrams);
            });

            var middleware = new FakeMiddleware(true);
            builder.UseMiddleware(middleware);
            IServer server = builder.Start();

            server.Shutdown();
        }

        private void MapDatagrams(DatagramFactory factory)
        {
            factory.RegisterDatagramType<MessageDatagram>(1);
        }
    }
}
