using System.IO;
using Compression.Queue;
using Infrastructure.Extensions;

namespace Compression.Files
{
    /// <summary>
    /// Breaks byte sequence from a stream into blocks.
    /// It is a producer in the producer-consumer pattern.
    /// </summary>
    internal class BlockReader : IBlockWorker<ByteBlock>
    {
        protected int CurrentBlockId;
        protected int BlockSize;

        public BlockReader(int blockSize)
        {
            BlockSize = blockSize;
        }

        protected BlockReader() { }

        /// <summary>
        /// Breaks byte sequence into blocks and puts them in the output queue.
        /// </summary>
        public void Start(Stream stream, ISharedQueue<ByteBlock> outputQueue)
        {
            do
            {
                outputQueue.Push(new ByteBlock
                {
                    Id = CurrentBlockId++,
                    Data = GetNextBlockData(stream),
                });
            } while (stream.HasNextByte());

            outputQueue.StopWrite();
        }

        protected virtual byte[] GetNextBlockData(Stream stream)
        {
            var nextBlockData = new byte[GetNextBlockSize(stream)];
            stream.Read(nextBlockData, 0, nextBlockData.Length);
            return nextBlockData;
        }

        protected virtual int GetNextBlockSize(Stream stream)
        {
            return stream.CountBytesRemaining() > BlockSize
                ? BlockSize
                : (int)stream.CountBytesRemaining();
        }
    }
}
