using System;

namespace GameServ
{
    [Flags]
    public enum ServerPolicy
    {
        None = 0,
        RequireAcknowledgement = 1,
    }
}