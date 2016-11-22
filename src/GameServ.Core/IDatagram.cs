using System.IO;

namespace GameServ
{
    public interface IDatagram : IPoolable
    {
        void Serialize(BinaryWriter serializer);

        void Deserialize(BinaryReader deserializer);

        bool IsMessageValid();
    }
}
