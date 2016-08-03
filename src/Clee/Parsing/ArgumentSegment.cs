using System.Linq;

namespace Clee.Parsing
{
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
}