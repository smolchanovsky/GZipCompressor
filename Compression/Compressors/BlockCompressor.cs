using System;
using Compression.Files;
using Compression.Queue;

namespace Compression.Compressors
{
    /// <summary>
    /// Compresses/decompresses blocks of bytes according to a given algorithm.
    /// It is a part of the producer-consumer pattern.
    /// </summary>
    internal class BlockCompressor : IBlockCompressor<ByteBlock>
    {
        private readonly Func<byte[], byte[]> _compressionFunc;

        public BlockCompressor(Func<byte[], byte[]> compressionFunc)
        {
            _compressionFunc = compressionFunc;
        }

        /// <summary>
        /// Compresses/decompresses blocks from the input queue and then puts them in the output queue.
        /// </summary>
        public void Start(ISharedQueue<ByteBlock> inputQueue, ISharedQueue<ByteBlock> outputQueue)
        {
            while (inputQueue.IsActive || inputQueue.Peek() != null)
            {
                var block = inputQueue.Pop();
                if (block == null)
                    continue;

                block.Data = _compressionFunc(block.Data);
                outputQueue.Push(block);
            }
            outputQueue.StopWrite();
        }
    }
}
