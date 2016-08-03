using System.Collections.Generic;

namespace Clee.Parsing
{
    public class ParseResult
    {
        public ParseResult(Path path, IEnumerable<Argument> arguments)
        {
            Path = path;
            Arguments = arguments;
        }

        public Path Path { get; private set; }
        public IEnumerable<Argument> Arguments { get; private set; }
    }
}