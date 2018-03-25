using System.Collections.Generic;
using Compression.Compressors;
using Compression.Files;
using Compression.Queue;
using Moq;
using NUnit.Framework;

namespace Compression.Tests.Compressor
{
    [TestFixture]
    public class BlockCompressorTests
    {
        private readonly BlockCompressor _blockCompressor;
        private readonly Mock<ISharedQueue<ByteBlock>> _inputSharedQueue;
        private readonly Mock<ISharedQueue<ByteBlock>> _outputSharedQueue;
        private Queue<ByteBlock> _inputQueue;
        private Queue<ByteBlock> _outputQueue;

        public BlockCompressorTests()
        {
            _inputSharedQueue = new Mock<ISharedQueue<ByteBlock>>();
            _inputSharedQueue
                .Setup(x => x.Pop())
                .Returns(() =>
                {
                    var result = _inputQueue.Dequeue();
                    if (_inputQueue.Count == 0)
                        _inputSharedQueue.Setup(x => x.IsActive).Returns(() => false);
                    return result;
                });

            _outputSharedQueue = new Mock<ISharedQueue<ByteBlock>>();
            _outputSharedQueue
                .Setup(x => x.Push(It.IsAny<ByteBlock>()))
                .Callback<ByteBlock>((block) =>
                {
                    _outputQueue.Enqueue(block);
                });

            _blockCompressor = new BlockCompressor((x) => x);
        }

        [Test]
        public void Start_ShiftFromInputQueueToOutputQueue_Success()
        {
            InitializeQueues();
            var expectedResult = new Queue<ByteBlock>(_inputQueue);

            _blockCompressor.Start(_inputSharedQueue.Object, _outputSharedQueue.Object);

            CollectionAssert.AreEqual(expectedResult, _outputQueue);
        }

        #region Initialization data for test
        private void InitializeQueues()
        {
            _inputQueue = new Queue<ByteBlock>(new[] { new ByteBlock { Id = 1 }, new ByteBlock { Id = 2 }, new ByteBlock { Id = 3 } });
            _inputSharedQueue
                .Setup(x => x.IsActive).Returns(() => true);

            _outputQueue = new Queue<ByteBlock>();
            _outputSharedQueue
                .Setup(x => x.IsActive).Returns(() => true);
        }
        #endregion
    }
}