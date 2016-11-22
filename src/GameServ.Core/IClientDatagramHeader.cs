using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServ
{
    public interface IClientDatagramHeader : IDatagramHeader
    {
        byte OSPlatform { get; }

        string OSVersion { get; }

        byte AppVersion { get; }
    }
}
