namespace GameServ
{
    public interface IServerDatagram : IDatagram
    {
        IServerDatagramHeader Header { get; }
    }
}