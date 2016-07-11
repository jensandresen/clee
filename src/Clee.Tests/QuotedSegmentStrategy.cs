namespace Clee.Tests
{
    public class QuotedSegmentStrategy : DefaultSegmentStrategy
    {
        protected override bool IsStopChar(string source, int offset)
        {
            return IsQuote(source, offset) &&
                   !IsEscapedQuote(source, offset);
        }

        protected override int FindEndOffset(string source, int beginOffset)
        {
            var endOffset = base.FindEndOffset(source, beginOffset);

            if (IsQuote(source, endOffset))
            {
                endOffset++;
            }

            return endOffset;
        }

        public static bool IsQuote(string source, int offset)
        {
            return source[offset] == '"';
        }

        private static bool IsEscapedQuote(string source, int offset)
        {
            return offset > 0 &&
                   source[offset] == '"' &&
                   source[offset - 1] == '\\';
        }
    }
}