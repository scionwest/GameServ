using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServ
{
    public interface IClientHeader : IDatagramHeader
    {
        byte MessageType { get; }

        int ClientId { get; }
    }
}
