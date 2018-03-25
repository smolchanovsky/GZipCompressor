using System.IO;
using Compression.Queue;

namespace Compression.Files
{
    /// <summary>
    /// Writes blocks of bytes to stream.
    /// It is consumer in the producer-consumer pattern.
    /// </summary>
    internal class BlockWriter : IBlockWorker<ByteBlock>
    {
        protected int CurrentBlockId;

        /// <summary>
        /// Writes blocks of bytes to the stream from the input queue.
        /// </summary>
        public void Start(Stream stream, ISharedQueue<ByteBlock> inputQueue)
        {
            while (inputQueue.IsActive || inputQueue.Peek() != null)
            {
                if (CurrentBlockId != inputQueue.Peek()?.Id)
                    continue;

                var block = inputQueue.Pop();
                stream.Write(block.Data, 0, block.Data.Length);
                ++CurrentBlockId;
            }
        }
    }
}
