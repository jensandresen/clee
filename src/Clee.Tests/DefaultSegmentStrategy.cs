namespace Clee.Tests
{
    public class DefaultSegmentStrategy
    {
        protected virtual bool IsStopChar(string source, int offset)
        {
            return char.IsWhiteSpace(source[offset]);
        }

        public Segment ExtractSegment(int beginOffset, string source)
        {
            var endOffset = FindEndOffset(source, beginOffset);
            var value = source.Substring(beginOffset, endOffset - beginOffset);

            return new Segment(
                value: value,
                beginOffset: beginOffset
                );
        }

        protected virtual int FindEndOffset(string source, int beginOffset)
        {
            var endOffset = beginOffset + 1;

            while (endOffset < source.Length && !IsStopChar(source, endOffset))
            {
                endOffset++;
            }

            return endOffset;
        }
    }
}