using System.IO;
using Compression.Queue;

namespace Compression.Files
{
    internal interface IBlockWorker<T> where T : class
    {
        void Start(Stream stream, ISharedQueue<T> queue);
    }
}