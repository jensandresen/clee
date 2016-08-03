namespace Clee.Parsing
{
    public class SegmentHelper
    {
        public static bool IsQuote(string source, int offset)
        {
            return source[offset] == '"';
        }

        public static bool IsEscapedQuote(string source, int offset)
        {
            return offset > 0 &&
                   source[offset] == '"' &&
                   source[offset - 1] == '\\';
        }
    }
}