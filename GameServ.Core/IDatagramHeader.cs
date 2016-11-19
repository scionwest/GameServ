namespace GameServ.Core
{
    public interface IDatagramHeader : IDatagram
    {
        byte SequenceNumber { get; }

        bool IsLastInSequence { get; }

        byte Channel { get; }

        DatagramPolicy Policy { get; }
    }
}
