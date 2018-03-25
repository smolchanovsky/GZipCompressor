using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class IntegrationTests
    {
        private readonly string _assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)?.Replace(@"file:\", "");
        private string ExePath => Path.GetFullPath(Path.Combine(_assemblyDirectory, @"..\..\..\Console\bin\Debug\GZipTest.exe"));
        private string EmptyFile => Path.Combine(_assemblyDirectory, "emptyFile.txt");
        private string SmallFile => Path.Combine(_assemblyDirectory, "smallFile.txt");
        private string LargeFile => Path.Combine(_assemblyDirectory, "largeFile.txt");
        private string CompressedFile => Path.Combine(_assemblyDirectory, "compressedFile.gz");
        private string DecompressedFile => Path.Combine(_assemblyDirectory, "decompressedFile.txt");


        [Test]
        public void CompressEmptyFile()
        {
            RunApp(GetCompressionArgs(EmptyFile, CompressedFile));
            RunApp(GetDecompressionArgs(CompressedFile, DecompressedFile));

            Assert.AreEqual(CalculateMD5(EmptyFile), CalculateMD5(DecompressedFile));
        }

        [Test]
        public void CompressSmallFile()
        {
            RunApp(GetCompressionArgs(SmallFile, CompressedFile));
            RunApp(GetDecompressionArgs(CompressedFile, DecompressedFile));

            Assert.AreEqual(CalculateMD5(SmallFile), CalculateMD5(DecompressedFile));
        }

        [Test]
        public void CompressLargeFile()
        {
            RunApp(GetCompressionArgs(LargeFile, CompressedFile));
            RunApp(GetDecompressionArgs(CompressedFile, DecompressedFile));

            Assert.AreEqual(CalculateMD5(LargeFile), CalculateMD5(DecompressedFile));
        }

        private void RunApp(string args)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = ExePath,
                Arguments = args,
            })?.WaitForExit();
        }

        private static string GetCompressionArgs(string inputFile, string outputFile)
        {
            return $"compress \"{inputFile}\" \"{outputFile}\"";
        }

        private static string GetDecompressionArgs(string inputFile, string outputFile)
        {
            return $"decompress \"{inputFile}\" \"{outputFile}\"";
        }

        private static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
                }
            }
        }
    }
}
