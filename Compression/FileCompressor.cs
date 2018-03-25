using System;
using System.IO;
using System.Linq;
using Compression.Compressors;
using Compression.Files;
using Compression.Queue;

namespace Compression
{
    /// <summary>
    /// Multi-threaded file compressor.
    /// </summary>
    public class FileCompressor
    {
        private const int BlockSize = 32 * 1024 * 1024;
        private const int MinTotalQueuesSize = 4;
        private readonly ICompressionProvider _compressionProvider = new GZipCompressionProvider();
        private readonly int _compressorsNumber;
        private readonly int _inputQueueSize;
        private readonly int _outputQueueSize;

        public FileCompressor(int threadsNumber, ulong memory)
        {
            if (GetTotalQueuesSize(memory) < MinTotalQueuesSize)
                throw new InvalidOperationException(Messages.NotEnoughMemory);

            _compressorsNumber = GetCompressorsNumber(threadsNumber);

            // inputQueue - 25% of the total size of queues
            _inputQueueSize = (int)(GetTotalQueuesSize(memory) * 0.25);
            // inputQueue - 75% of the total size of queues
            _outputQueueSize = (int)(GetTotalQueuesSize(memory) * 0.75);
        }

        /// <param name="compressionProvider">Compression algorithm provider.</param>
        /// <param name="threadsNumber">Number of available threads.</param>
        /// <param name="memory">The maximum size of the used RAM.</param>
        public FileCompressor(ICompressionProvider compressionProvider, int threadsNumber, ulong memory) : this(threadsNumber, memory)
        {
            _compressionProvider = compressionProvider;
        }

        /// <summary>
        /// Compresses the file.
        /// </summary>
        /// <param name="inputFile">File for compression.</param>
        /// <param name="outputFile">Compressed file.</param>
        public void Compress(string inputFile, string outputFile)
        {
            CompressorAction(
                inputFile,
                outputFile,
                new BlockReader(BlockSize),
                new BlockCompressor(_compressionProvider.Compress),
                new BlockWriter());
        }

        /// <summary>
        /// Decompresses the file.
        /// </summary>
        /// <param name="inputFile">File for decompression.</param>
        /// <param name="outputFile">Decompressed file</param>
        public void Decompress(string inputFile, string outputFile)
        {
            CompressorAction(
                inputFile,
                outputFile,
                new CompressedBlockReader(_compressionProvider.BlockHeader, BlockSize),
                new BlockCompressor(_compressionProvider.Decompress),
                new BlockWriter());
        }

        private void CompressorAction(
            string inputFile, 
            string outputFile,
            IBlockWorker<ByteBlock> blockReader,
            IBlockCompressor<ByteBlock> blockCompressor,
            IBlockWorker<ByteBlock> blockWriter)
        {
            var inputQueue = new SharedQueue<ByteBlock>(new SimpleQueue<ByteBlock>(), _inputQueueSize);
            var outputQueue = new SharedQueue<ByteBlock>(new SortedQueue<ByteBlock>(), _outputQueueSize, _compressorsNumber);

            using (var inputStream = File.Open(Path.GetFullPath(inputFile), FileMode.Open, FileAccess.Read))
            {
                ThreadStarter.StartThread(() => blockReader.Start(inputStream, inputQueue));

                foreach (var _ in Enumerable.Range(0, _compressorsNumber))
                {
                    ThreadStarter.StartThread(() => blockCompressor.Start(inputQueue, outputQueue));
                }

                using (var outputStream = File.Open(Path.GetFullPath(outputFile), FileMode.Create, FileAccess.Write))
                {
                    blockWriter.Start(outputStream, outputQueue);
                }
            }
        }

        /// <summary>
        /// Calculates the number of threads directly for compression (without IO threads).
        /// </summary>
        /// <param name="threadsNumber">Number of available threads.</param>
        /// <returns>Number of threads for compression</returns>
        private static int GetCompressorsNumber(int threadsNumber)
        {
            const int ioThreadsNumber = 2;
            const int minCompressorsNumber = 1;

            return threadsNumber > ioThreadsNumber
                ? threadsNumber - ioThreadsNumber
                : minCompressorsNumber;
        }

        /// <summary>
        /// Calculates the total size of queues based on the allocated memory.
        /// </summary>
        /// <param name="memory">Allow memory</param>
        /// <returns>Total size of queues</returns>
        private static int GetTotalQueuesSize(ulong memory)
        {
            const ulong queueNumber = 2;
            return (int) (memory / queueNumber / BlockSize > Int32.MaxValue
                ? Int32.MaxValue
                : memory / queueNumber / BlockSize);
        }
    }
}
