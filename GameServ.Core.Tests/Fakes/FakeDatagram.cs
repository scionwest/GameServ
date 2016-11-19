using System;
using System.IO;

namespace GameServ.Core.Tests.Fakes
{
    public class FakeDatagram : IDatagram
    {
        public bool IsSafeForReuse;

        public void SetAsUnsafeForReuse() => this.IsSafeForReuse = false;

        public void PrepareForReuse()
        {
            this.IsSafeForReuse = true;
        }

        public void Deserialize(BinaryReader deserializer)
        {
            this.IsSafeForReuse = false;
        }

        public bool IsMessageValid()
        {
            this.IsSafeForReuse = false;
            return true;
        }

        public void Serialize(BinaryWriter serializer)
        {
            this.IsSafeForReuse = false;
        }
    }
}
