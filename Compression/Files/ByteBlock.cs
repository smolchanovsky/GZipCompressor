using Infrastructure.Interfaces;

namespace Compression.Files
{
    internal class ByteBlock : IIdentifiable
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
    }
}
