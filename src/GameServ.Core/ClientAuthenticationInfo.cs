using System.Net;

namespace GameServ
{
    public struct ClientAuthenticationInfo
    {
        public ClientAuthenticationInfo(IPEndPoint endPoint, string authenticationToken, string clientId)
        {
            this.EndPoint = endPoint;
            this.AuthenticationToken = authenticationToken;
            this.ClientId = clientId;
        }

        public IPEndPoint EndPoint { get; }

        public string AuthenticationToken { get; }

        public string ClientId { get; }
    }
}
