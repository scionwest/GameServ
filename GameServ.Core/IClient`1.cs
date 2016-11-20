using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServ
{
    public interface IClient<TOwner> : IClient
    {
        TOwner Owner { get; }
    }
}
