using System.Collections.Generic;
using System.Linq;
using Clee.Routing;

namespace Clee.Parsing
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
            return new ArgumentReader(segments).ReadAll();
        }

        private Path GetPathFrom(IEnumerable<Segment> segments)
        {
            var firstSegment = segments.FirstOrDefault();
            if (firstSegment != null && firstSegment.Value.StartsWith("\""))
            {
                throw new ParseException(firstSegment.BeginOffset, "Unsupported character \" in command name.");
            }

            var pathCandidates = segments
                .TakeWhile(x => char.IsLetter(x.Value[0]))
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