using System.Collections.Generic;
using System.IO;
using System.Linq;
using Compression.Files;
using Compression.Queue;
using Moq;
using NUnit.Framework;

namespace Compression.Tests.Files
{
    [TestFixture]
    public class CompressedBlockReaderTests
    {
        private static readonly byte[] BlockHeader = {0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00};
        private readonly CompressedBlockReader _compressedBlockReader;
        private readonly List<byte[]> _testBlocks;
        private readonly Mock<ISharedQueue<ByteBlock>> _outputSharedQueue;
        private Queue<ByteBlock> _outputQueue;

        public CompressedBlockReaderTests()
        {
            _testBlocks = new List<byte[]>
            {
                BlockHeader.Concat(new byte[] { 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, }).ToArray(),
                BlockHeader.Concat(new byte[] { 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, }).ToArray(),
                BlockHeader.Concat(new byte[] { }).ToArray(),
            };

            _outputSharedQueue = new Mock<ISharedQueue<ByteBlock>>();
            _outputSharedQueue
                .Setup(x => x.IsActive).Returns(() => true);
            _outputSharedQueue
                .Setup(x => x.Push(It.IsAny<ByteBlock>()))
                .Callback<ByteBlock>((block) =>
                {
                    _outputQueue.Enqueue(block);
                });

            _compressedBlockReader = new CompressedBlockReader(BlockHeader, uncompressedBlockSize: _testBlocks.First().Length);
        }

        [Test]
        public void Start_CorrectStream_CorrectBlocks()
        {
            InitializeQueues();
            var streamBytes = _testBlocks.SelectMany(x => x).ToArray();

            using (var memoryStream = new MemoryStream(streamBytes))
            {
                _compressedBlockReader.Start(memoryStream, _outputSharedQueue.Object);
            }

            Assert.AreEqual(expected: _testBlocks.Count, actual: _outputQueue.Count);
            foreach (var expectedBlock in _testBlocks)
                CollectionAssert.AreEqual(expectedBlock, _outputQueue.Dequeue().Data);
        }

        #region Initialization data for test
        private void InitializeQueues()
        {
            _outputQueue = new Queue<ByteBlock>();
            _outputSharedQueue
                .Setup(x => x.IsActive).Returns(() => true);
        }
        #endregion
    }
}
