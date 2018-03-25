namespace Console.Commands
{
    internal interface ICommand
    {
        string Name { get; set; }
        string ShortName { get; set; }
    }
}