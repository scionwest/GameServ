namespace GameServ
{
    public interface IDatagramHeader : IDatagram
    {
        byte SequenceNumber { get; }

        bool IsLastInSequence { get; }

        long TimeStamp { get; }

        byte Channel { get; }

        DatagramPolicy Policy { get; }

        byte MessageType { get; }

        int ClientId { get; }
    }
}
