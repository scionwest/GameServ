using System.Net;

namespace GameServ
{
    public interface IAuthenticator
    {
        ClientAuthenticationInfo Authenticate(IPEndPoint endPoint);
    }
}
