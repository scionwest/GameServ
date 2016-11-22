using System;
using System.Collections.Generic;
using System.Text;

namespace GameServ.Server
{
    public class ServerBuilder : IServerBuilder
    {
        private List<IMiddleware> registeredMiddleware;
        private ServerConfiguration serverConfiguration = new ServerConfiguration();

        public IServer Start()
        {
            var server = new SocketEventArgsServer();
            this.StartServer(server);
            return server;
        }

        public IServerBuilder UseMiddleware<TMiddleware>(TMiddleware middleware) where TMiddleware : IMiddleware
        {
            if (this.registeredMiddleware == null)
            {
                this.registeredMiddleware = new List<IMiddleware>();
            }

            this.registeredMiddleware.Add(middleware);
            return this;
        }

        public IServerBuilder Configure(Action<ServerConfiguration> configuration)
        {
            configuration(this.serverConfiguration);
            return this;
        }

        private void StartServer(SocketEventArgsServer server)
        {
            if (this.registeredMiddleware != null)
            {
                server.UseMiddleware(this.registeredMiddleware);    
            }

            server.ServerPort = this.serverConfiguration.Port;
            server.Start();
        }
    }
}
