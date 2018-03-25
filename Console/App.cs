using System;
using Compression;
using Console.Commands;

namespace Console
{
    internal class App
    {
        private static readonly CommandParser CommandParser;

        static App()
        {
            CommandParser = new CommandParser();
        }

        /// <param name="args">Command-line args</param>
        /// <param name="processorCount">Number of processors on the current machine.</param>
        /// <param name="memory">Size of RAM on the current machine.</param>
        public static void Run(string[] args, int processorCount, ulong memory)
        {
            var compressor = new FileCompressor(processorCount, GetUsedMemory(memory));
            try
            {
                var command = CommandParser.Parse(args);
                switch (command)
                {
                    case CompressCommand compressCommand:
                        compressor.Compress(compressCommand.InputFilePath, compressCommand.OutputFilePath);
                        break;
                    case DecompressCommand decompressCommand:
                        compressor.Decompress(decompressCommand.InputFilePath, decompressCommand.OutputFilePath);
                        break;
                    default:
                        throw new InvalidOperationException(Messages.CommandNotSupported);
                }
                System.Console.WriteLine(Messages.SuccessOperation);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            System.Console.ReadKey();
        }

        public static ulong GetUsedMemory(ulong availablePhysicalMemory)
        {
            const ulong memoryLimits32Bit = 2000000000; // ~ 2Gb

            // If the application is assembled as 32 bit then the memory limit is 2 Gb
            var usedMemory = availablePhysicalMemory;
            if (IntPtr.Size == 4)
                usedMemory = availablePhysicalMemory > memoryLimits32Bit
                ? memoryLimits32Bit
                : availablePhysicalMemory;
            return usedMemory;
        }
    }
}