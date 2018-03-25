namespace Console.Commands
{
    /// <summary>
    /// Command contains all the necessary data for decompression.
    /// </summary>
    internal class DecompressCommand : Command
    {
        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }
    }
}
