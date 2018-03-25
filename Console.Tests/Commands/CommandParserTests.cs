using System;
using Console.Commands;
using NUnit.Framework;

namespace Console.Tests.Commands
{
    [TestFixture]
    public class CommandParserTests
    {
        private readonly CommandParser _commandParser;

        public CommandParserTests()
        {
            _commandParser = new CommandParser();
        }

        [TestCase(new object[] {"compress", "inputFile.txt", "outputFile.txt"}, ExpectedResult = typeof(CompressCommand))]
        [TestCase(new object[] { "decompress", "inputFile.txt", "outputFile.txt" }, ExpectedResult = typeof(DecompressCommand))]
        public Type Parse_CorrectArgs_ResturnCorrectCommand(object[] objArgs)
        {
            var args = ConvertToArgs(objArgs);

            var command = _commandParser.Parse(args);
            return command.GetType();
        }

        [TestCase(new object[] { }, ExpectedResult = typeof(ArgumentException))]
        [TestCase(new object[] { "compress1", "inputFile.txt", "outputFile.txt" }, ExpectedResult = typeof(InvalidOperationException))]
        [TestCase(new object[] { "incorrectCommand", "inputFile.txt", "outputFile.txt" }, ExpectedResult = typeof(InvalidOperationException))]
        public Type Parse_IncorrectCommand_ThrowException(object[] objArgs = null)
        {
            var args = ConvertToArgs(objArgs);

            try
            {
                var command = _commandParser.Parse(args);
                return command.GetType();
            }
            catch (Exception ex)
            {
                return ex.GetType();
            }
        }

        [TestCase(new object[] { "compress" }, ExpectedResult = typeof(InvalidOperationException))]
        [TestCase(new object[] { "decompress", "inputFile.txt" }, ExpectedResult = typeof(InvalidOperationException))]
        public Type Parse_IncompleteArgs_ThrowException(object[] objArgs)
        {
            var args = ConvertToArgs(objArgs);

            try
            {
                var command = _commandParser.Parse(args);
                return command.GetType();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<IndexOutOfRangeException>(ex.InnerException);
                return ex.GetType();
            }
        }

        [TestCase(new object[] { "compress", "inputFile.txt", "outputFile.txt" })]
        public void Parse_CompressArgs_CorrectCompressCommand(object[] objArgs)
        {
            var args = ConvertToArgs(objArgs);
            var command = _commandParser.Parse(args) as CompressCommand;

            Assert.IsInstanceOf<CompressCommand>(command);
            Assert.AreEqual(args[1], command.InputFilePath);
            Assert.AreEqual(args[2], command.OutputFilePath);
        }

        [TestCase(new object[] { "decompress", "inputFile.txt", "outputFile.txt" })]
        public void Parse_DecompressArgs_CorrectDecompressCommand(object[] objArgs)
        {
            var args = ConvertToArgs(objArgs);
            var command = _commandParser.Parse(args) as DecompressCommand;

            Assert.IsInstanceOf<DecompressCommand>(command);
            Assert.AreEqual(args[1], command.InputFilePath);
            Assert.AreEqual(args[2], command.OutputFilePath);
        }

        private static string[] ConvertToArgs(object[] objArgs)
        {
            return objArgs != null
                ? Array.ConvertAll(objArgs, x => x.ToString())
                : null;
        }
    }
}