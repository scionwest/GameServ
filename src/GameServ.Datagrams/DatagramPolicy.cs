using System;

namespace GameServ
{
    [Flags]
    public enum DatagramPolicy
    {
        None = 0,
        SequencedMessage = 1,
        AcknoweldgementRequired = 2,
        CompletedSequenceRequired = 4,
        IgnoringNotAllowed = 8,
    }
}