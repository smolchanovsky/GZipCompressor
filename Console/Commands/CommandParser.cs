using System;
using System.Collections.Generic;
using System.Linq;

namespace Console.Commands
{
    /// <summary>
    /// Parser command-line arguments.
    /// </summary>
    internal class CommandParser
    {
        private ICollection<ICommand> _commands;

        public CommandParser()
        {
            InitCommands();
        }

        /// <summary>
        /// Parses command-line arguments in the application specific command.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ICommand Parse(string[] args)
        {
            if (args == null)
                throw new ArgumentException(Messages.CommandNotSpecified, nameof(args));

            var commandName = args.FirstOrDefault();
            if (commandName == null)
                throw new InvalidOperationException(Messages.CommandNotSpecified);

            var command = _commands.FirstOrDefault(x => x.Name == commandName);
            if (command == null)
                throw new InvalidOperationException(Messages.CommandNotFound);

            try
            {
                switch (command)
                {
                    case CompressCommand compressCommand:
                        compressCommand.InputFilePath = args[1];
                        compressCommand.OutputFilePath = args[2];
                        break;
                    case DecompressCommand decompressCommand:
                        decompressCommand.InputFilePath = args[1];
                        decompressCommand.OutputFilePath = args[2];
                        break;
                    default:
                        throw new InvalidOperationException(Messages.CommandNotSupported);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new InvalidOperationException(Messages.NotEnoughOptions, ex);
            }

            return command;
        }

        private void InitCommands()
        {
            _commands = new List<ICommand>
            {
                new CompressCommand
                {
                    Name = "compress",
                    ShortName = "c",
                },
                new DecompressCommand
                {
                    Name = "decompress",
                    ShortName = "d",
                },
            };
        }
    }
}
