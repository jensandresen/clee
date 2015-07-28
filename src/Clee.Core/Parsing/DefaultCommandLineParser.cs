using System.Collections.Generic;

namespace Clee.Parsing
{
    public class DefaultCommandLineParser : ICommandLineParser
    {
        public string ExtractCommandNameFrom(string input)
        {
            return CommandLineParser.ExtractCommandNameFrom(input);
        }

        public IEnumerable<Argument> ExtractArgumentsFrom(string input)
        {
            return CommandLineParser.ExtractArgumentsFrom(input);
        }

        public IEnumerable<Argument> ParseArguments(string input)
        {
            return CommandLineParser.ParseArguments(input);
        }
    }
}