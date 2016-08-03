namespace Clee.Parsing
{
    public class DefaultSegmentStrategy : SegmentStrategyBase
    {
        protected virtual bool IsStopChar(string source, int offset)
        {
            return char.IsWhiteSpace(source[offset]);
        }

        protected override bool IsValidSegmentStart(int beginOffset, string source)
        {
            return IsValidSegmentCharacter(source, beginOffset);
        }

        private static bool IsValidSegmentCharacter(string source, int endOffset)
        {
            return source[endOffset] != '"';
        }

        protected override int FindEndOffset(string source, int beginOffset)
        {
            var endOffset = beginOffset + 1;

            while (endOffset < source.Length && !IsStopChar(source, endOffset))
            {
                if (!IsValidSegmentCharacter(source, endOffset))
                {
                    throw new SegmentException(endOffset, source);
                }

                endOffset++;
            }

            return endOffset;
        }
    }
}