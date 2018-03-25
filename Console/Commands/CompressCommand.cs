namespace Console.Commands
{
    /// <summary>
    /// Command contains all the necessary data for compression.
    /// </summary>
    internal class CompressCommand : Command
    {
        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }
    }
}
