using System.IO;

namespace GameServ.Core
{
    public interface IDatagram : IPoolable
    {
        void Serialize(BinaryWriter serializer);

        void Deserialize(BinaryReader deserializer);

        bool IsMessageValid();
    }
}
