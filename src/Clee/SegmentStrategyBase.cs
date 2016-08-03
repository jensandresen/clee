namespace Clee
{
    public abstract class SegmentStrategyBase : ISegmentStrategy
    {
        protected abstract bool IsValidSegmentStart(int beginOffset, string source);
        protected abstract int FindEndOffset(string source, int beginOffset);

        public virtual Segment ExtractSegment(int beginOffset, string source)
        {
            if (!IsValidSegmentStart(beginOffset, source))
            {
                throw new SegmentException(beginOffset, source);
            }

            var endOffset = FindEndOffset(source, beginOffset);
            var value = source.Substring(beginOffset, endOffset - beginOffset);

            return new Segment(
                value: value,
                beginOffset: beginOffset
                );
        }
    }
}