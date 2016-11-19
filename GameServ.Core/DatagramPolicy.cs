﻿using System;

namespace GameServ.Core
{
    [Flags]
    public enum DatagramPolicy
    {
        SequencedMessage = 1,
        AcknoweldgementRequired = 2,
        CompletedSequenceRequired = 4,
        IgnoringNotAllowed = 8,
    }
}