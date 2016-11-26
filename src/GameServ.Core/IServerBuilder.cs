using System;
using System.Collections.Generic;
using System.Text;

namespace GameServ
{
    public interface IServerBuilder
    {
        IServerBuilder UseMiddleware<TMiddleware>(TMiddleware middleware) where TMiddleware : IMiddleware;

        void StartListening();
    }
}
