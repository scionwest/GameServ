using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServ.Server
{
    public class ServerBuilder : IServerBuilder
    {
        private List<IMiddleware> registeredMiddleware;
        private ServerConfiguration serverConfiguration = new ServerConfiguration();

        public void StartListening()
        {
            var server = new SocketListener(
                this.registeredMiddleware ?? Enumerable.Empty<IMiddleware>(),
                this.serverConfiguration);
            this.StartServer(server);
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

        private void StartServer(SocketListener server)
        {
            server.Start();
        }
    }
}
