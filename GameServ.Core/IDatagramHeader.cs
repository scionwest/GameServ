namespace GameServ.Core
{
    public interface IDatagramHeader
    {
        byte SequenceNumber { get; }

        bool IsLastInSequence { get; }

        byte Channel { get; }

        DatagramPolicy Policy { get; }
    }
}
