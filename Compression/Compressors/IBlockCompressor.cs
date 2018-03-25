using Compression.Queue;

namespace Compression.Compressors
{
    internal interface IBlockCompressor<T> where T : class
    {
        void Start(ISharedQueue<T> inputQueue, ISharedQueue<T> outputQueue);
    }
}