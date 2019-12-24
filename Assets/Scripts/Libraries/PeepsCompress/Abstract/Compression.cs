using System.IO;

namespace PeepsCompress
{
    public abstract class Compression
    {
        public abstract byte[] decompress(BinaryReader br);
    }
}
