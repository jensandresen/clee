using System.Collections.Generic;
using System.Linq;

namespace Clee.Tests
{
    public class GetOptStyleParser
    {
        public ParseResult Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            var segments = GetSegmentsFrom(input);
            var path = GetPathFrom(segments);
            var arguments = GetArgumentsFrom(segments);

            return new ParseResult(path, arguments);
        }

        private IEnumerable<Argument> GetArgumentsFrom(IEnumerable<Segment> segments)
        {
            var candidates = new Queue<Segment>(segments.SkipWhile(x => !x.Value.StartsWith("-")));

            var result = new LinkedList<Argument>();

            while (candidates.Count > 0)
            {
                var seg = candidates.Dequeue();

                if (!seg.Value.StartsWith("-"))
                {
                    throw new ParseException(seg.BeginOffset, "Unknown argument definition.");
                }

                var argumentName = seg.Value.TrimStart('-');
                var argumentValue = "";

                if (candidates.Count > 0)
                {
                    var nextSegment = candidates.Peek();
                    if (nextSegment != null && !nextSegment.Value.StartsWith("-"))
                    {
                        var temp = candidates.Dequeue();
                        argumentValue = temp.Value;
                        argumentValue = argumentValue.Trim('"');
                        argumentValue = argumentValue.Replace("\\\"", "\"");
                    }
                }

                result.AddLast(new Argument(
                    name: argumentName,
                    value: argumentValue
                    ));
            }

            return result;
        }

        private Path GetPathFrom(IEnumerable<Segment> segments)
        {
            var pathCandidates = segments
                .TakeWhile(x => !x.Value.StartsWith("-"))
                .ToArray();

            if (pathCandidates.Length == 0)
            {
                var offset = segments.First().BeginOffset;
                throw new ParseException(offset, "Command definition is missing.");
            }

            var path = new Path(pathCandidates[0].Value);

            foreach (var seg in pathCandidates.Skip(1))
            {
                path.AddSegment(seg.Value);
            }

            return path;
        }

        private Segment[] GetSegmentsFrom(string input)
        {
            var segmentsReader = new SegmentsReader();
            
            return segmentsReader
                .ReadAllFrom(input)
                .ToArray();
        }
    }
}