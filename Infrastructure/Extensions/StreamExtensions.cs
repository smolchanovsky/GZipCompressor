using System.IO;

namespace Infrastructure.Extensions
{
    public static class StreamExtensions
    {
        public static bool HasNextByte(this Stream stream)
        {
            return stream.Position != stream.Length;
        }

        public static long CountBytesRemaining(this Stream stream)
        {
            return stream.Length - stream.Position;
        }
    }
}
