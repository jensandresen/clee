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

    internal class ArgumentReader
    {
        private readonly Segment[] _segments;

        public ArgumentReader(IEnumerable<Segment> segments)
        {
            _segments = segments
                .SkipWhile(x => char.IsLetter(x.Value[0]))
                .ToArray();
        }

        public IEnumerable<Argument> ReadAll()
        {
            var result = new LinkedList<Argument>();

            var index = 0;

            while (index < _segments.Length)
            {
                var argumentSegment = new ArgumentSegment(_segments[index]);
                argumentSegment.Validate();

                index++;

                ValueSegment valueSegment;
                if (TryCreateValueSegment(index, out valueSegment))
                {
                    index++;
                }

                if (argumentSegment.IsMulti)
                {
                    var list = ConvertToMultipleArguments(argumentSegment.Name, valueSegment.Value);

                    foreach (var argument in list)
                    {
                        result.AddLast(argument);
                    }
                }
                else
                {
                    result.AddLast(new Argument(
                        name: argumentSegment.Name,
                        value: valueSegment.Value
                        ));
                }
            }

            return result;
        }

        private IEnumerable<Argument> ConvertToMultipleArguments(string argumentName, string argumentValue)
        {
            for (var index = 0; index < argumentName.Length; index++)
            {
                var isLastArgument = index == argumentName.Length - 1;

                var name = argumentName[index];
                var value = "";

                if (isLastArgument)
                {
                    value = argumentValue;
                }

                yield return new Argument(
                    name: new string(name, 1),
                    value: value
                    );
            }
        }

        private bool TryCreateValueSegment(int index, out ValueSegment result)
        {
            if (index < _segments.Length)
            {
                var valueSegment = new ValueSegment(_segments[index]);

                if (valueSegment.IsValid)
                {
                    result = valueSegment;
                    return true;
                }
            }
            
            result = ValueSegment.NoValue;
            return false;
        }
    }

    internal class ArgumentSegment
    {
        private readonly Segment _segment;

        public ArgumentSegment(Segment segment)
        {
            _segment = segment;
        }

        private bool IsShort
        {
            get { return PrefixCount == 1; }
        }

        private bool IsLong
        {
            get { return PrefixCount == 2; }
        }

        public bool IsMulti
        {
            get { return IsShort && Name.Length > 1; }
        }

        private int PrefixCount
        {
            get
            {
                var dashes = _segment
                    .Value
                    .TakeWhile(x => x.Equals('-'));

                return dashes.Count();
            }
        }

        public string Name
        {
            get { return _segment.Value.TrimStart('-'); }
        }

        private int BeginOffset
        {
            get { return _segment.BeginOffset; }
        }

        public void Validate()
        {
            if (!(IsShort || IsLong))
            {
                throw new ParseException(BeginOffset, "Unrecognized argument definition. Please use one or two dashes (- or --) to begin an argument definition followed by a valid argument name.");
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ParseException(BeginOffset + PrefixCount, "Argument name is missing.");
            }
        }
    }

    internal class ValueSegment
    {
        public static readonly ValueSegment NoValue = new ValueSegment(null);

        private readonly Segment _segment;

        public ValueSegment(Segment segment)
        {
            _segment = segment;
        }

        public string Value
        {
            get
            {
                if (_segment == null)
                {
                    return "";
                }

                return _segment
                    .Value
                    .Trim('"')
                    .Replace("\\\"", "\"");
            }
        }

        public bool IsValid
        {
            get
            {
                return !_segment.Value.StartsWith("-");
            }
        }
    }
}