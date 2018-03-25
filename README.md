# GZipCompressor

Console program for multi-threaded block compression and decompression of files using System.IO.Compression.GzipStream.
For compression, the source file is divided into blocks of the same size. Each block compresses and writes to the output file independently of the other blocks.
