using System.IO;

namespace GameServ
{
    public interface IDatagram
    {
        void Serialize(BinaryWriter serializer);

        void Deserialize(BinaryReader deserializer);

        void Reset();

        bool IsValid();
    }
}
