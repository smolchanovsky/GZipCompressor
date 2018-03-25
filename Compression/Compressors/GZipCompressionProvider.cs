using System.IO;
using System.IO.Compression;

namespace Compression.Compressors
{
    /// <summary>
    /// Implements GZip compression.
    /// </summary>
    public class GZipCompressionProvider : ICompressionProvider
    {
        /// <summary>
        /// First 10 bytes of block - GZip header.
        /// </summary>
        // +---+---+---+---+---+---+---+---+---+---+
        // |ID1|ID2|CM |FLG|     MTIME     |XFL|OS | (more-->)
        // +---+---+---+---+---+---+---+---+---+---+
        public byte[] BlockHeader { get; } = {0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00};

        /// <summary>
        /// Compresses byte sequence using <see cref="GZipStream"/>.
        /// </summary>
        /// <param name="bytes">Byte sequence for compression.</param>
        /// <returns>Compressed data.</returns>
        public byte[] Compress(byte[] bytes)
        {
            using (var inputMemoryStream = new MemoryStream(bytes))
            {
                using (var outputMemoryStream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(outputMemoryStream, CompressionMode.Compress))
                    {
                        inputMemoryStream.CopyTo(gzipStream);
                    }
                    return outputMemoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Decompresses byte sequence using <see cref="GZipStream"/>.
        /// </summary>
        /// <param name="bytes">Byte sequence for decompression.</param>
        /// <returns>Decompressed data.</returns>
        public byte[] Decompress(byte[] bytes)
        {
            using (var inputMemoryStream = new MemoryStream(bytes))
            {
                using (var outputMemoryStream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(inputMemoryStream, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(outputMemoryStream);
                    }
                    return outputMemoryStream.ToArray();
                }
            }
        }
    }
}
