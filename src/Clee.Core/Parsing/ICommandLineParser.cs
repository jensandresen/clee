using System.Collections.Generic;

namespace Clee.Parsing
{
    public interface ICommandLineParser
    {
        string ExtractCommandNameFrom(string input);
        IEnumerable<Argument> ExtractArgumentsFrom(string input);
        IEnumerable<Argument> ParseArguments(string input);
    }
}