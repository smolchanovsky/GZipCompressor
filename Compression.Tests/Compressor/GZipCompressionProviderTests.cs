using Compression.Compressors;
using NUnit.Framework;

namespace Compression.Tests.Compressor
{
    [TestFixture]
    public class GZipCompressionProviderTests
    {
        private readonly GZipCompressionProvider _gzipCompressionProvider;
        private readonly byte[] _testBytes;
        private readonly byte[] _compressedTestBytes;

        public GZipCompressionProviderTests()
        {
            _testBytes = new byte[] { 1, 2, 3, };
            _compressedTestBytes = new byte[] {31, 139, 8, 0, 0, 0, 0, 0, 4, 0, 99, 100, 98, 6, 0, 29, 128, 188, 85, 3, 0, 0, 0 };
            _gzipCompressionProvider = new GZipCompressionProvider();
        }

        [Test]
        public void Compress_CompressBytes_Success()
        {
            var result = _gzipCompressionProvider.Compress(_testBytes);

            CollectionAssert.AreEqual(_compressedTestBytes, result);
        }

        [Test]
        public void Decompress_DecompressBytes_Success()
        {
            var result = _gzipCompressionProvider.Decompress(_compressedTestBytes);
            
            CollectionAssert.AreEqual(_testBytes, result);
        }
    }
}
