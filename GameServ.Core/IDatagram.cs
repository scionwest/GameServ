using System.IO;

namespace GameServ.Core
{
    public interface IDatagram
    {
        void Serialize(BinaryWriter serializer);

        void Deserialzie(BinaryReader deserializer);

        bool IsMessageValid();

        void PrepareForReuse();
    }
}
