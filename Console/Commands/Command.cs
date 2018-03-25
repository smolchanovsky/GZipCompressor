namespace Console.Commands
{
    internal abstract class Command : ICommand
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}
