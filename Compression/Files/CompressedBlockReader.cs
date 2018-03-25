using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure.Extensions;

namespace Compression.Files
{
    /// <summary>
    /// Breaks the bytes from a stream into compressed blocks.
    /// It is a producer in the producer-consumer pattern.
    /// </summary>
    internal class CompressedBlockReader : BlockReader
    {
        private readonly byte[] _blockHeader;

        public CompressedBlockReader(byte[] blockHeader, int uncompressedBlockSize)
        {
            _blockHeader = blockHeader;
            // There is no direct correlation between size of uncompressed block and size of compressed block.
            // So takes for the truth that a compressed block can't be 3 times larger than a decompressed block.
            BlockSize = uncompressedBlockSize * 3;
        }

        protected override byte[] GetNextBlockData(Stream stream)
        {
            // For empty files
            if (stream.Length == 0)
                return new byte[0];

            var blockCandidate = new byte[GetNextBlockSize(stream)];
            stream.Read(blockCandidate, 0, blockCandidate.Length);
            using (var blockCandidatReader = new MemoryStream(blockCandidate))
            {
                var header = new byte[_blockHeader.Length];
                blockCandidatReader.Read(header, 0, header.Length);
                if (!IsHeader(header))
                    throw new InvalidOperationException(Messages.BlockHeaderNotFound);

                // Confirmed data of the block
                var blockData = new List<byte>(header);
                // The buffer contains last N elements where N is the block header size
                var byteBuffer = new Queue<byte>(_blockHeader.Length);
                // The number of bytes in the buffer that match the block header
                var matchHeaderBytesCount = 0;
                while (blockCandidatReader.HasNextByte())
                {
                    var currentByte = Convert.ToByte(blockCandidatReader.ReadByte());

                    byteBuffer.Enqueue(currentByte);

                    if (IsMatcheHeaderElement(currentByte, matchHeaderBytesCount))
                        ++matchHeaderBytesCount;
                    else
                        matchHeaderBytesCount = 0;

                    // If true then byteBuffer is the next block header
                    if (matchHeaderBytesCount == _blockHeader.Length)
                    {
                        // Shifts the position of the stream by the number of bytes that have been read but not written to the blockData
                        stream.Position -= blockCandidatReader.Length - blockData.Count;
                        return blockData.ToArray();
                    }

                    if (byteBuffer.Count == _blockHeader.Length)
                        blockData.Add(byteBuffer.Dequeue());
                }

                blockData.AddRange(byteBuffer);
                return blockData.ToArray();
            }
        }

        private bool IsHeader(IReadOnlyCollection<byte> headerCandidate)
        {
            if (headerCandidate is null)
                return false;
            if (headerCandidate.Count < _blockHeader.Length)
                return false;

            return _blockHeader.SequenceEqual(headerCandidate);
        }

        private bool IsMatcheHeaderElement(byte @byte, int counter)
        {
            return _blockHeader[counter] == @byte;
        }
    }
}
