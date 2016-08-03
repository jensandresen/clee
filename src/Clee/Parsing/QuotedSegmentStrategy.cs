namespace Clee.Parsing
{
    public class QuotedSegmentStrategy : SegmentStrategyBase
    {
        private static bool IsStopChar(string source, int offset)
        {
            return SegmentHelper.IsQuote(source, offset) &&
                   !SegmentHelper.IsEscapedQuote(source, offset);
        }

        protected override bool IsValidSegmentStart(int beginOffset, string source)
        {
            return SegmentHelper.IsQuote(source, beginOffset);
        }

        protected override int FindEndOffset(string source, int beginOffset)
        {
            var endOffset = beginOffset + 1;

            while (endOffset < source.Length && !IsStopChar(source, endOffset))
            {
                endOffset++;
            }

            if (endOffset >= source.Length)
            {
                throw new SegmentException(endOffset, source);
            }

            if (SegmentHelper.IsQuote(source, endOffset))
            {
                endOffset++;
            }

            return endOffset;
        }
    }
}