namespace Compression.Compressors
{
    public interface ICompressionProvider
    {
        byte[] BlockHeader { get; }
        byte[] Compress(byte[] bytes);
        byte[] Decompress(byte[] bytes);
    }
}
