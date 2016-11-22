namespace GameServ
{
    public interface IMiddleware
    {
        bool EvaluateClientDatagram(byte[] buffer, IClientDatagram clientDatagram);
    }
}