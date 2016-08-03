namespace Clee
{
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