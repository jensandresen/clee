using System.Collections.Generic;
using System.Linq;

namespace Clee.Tests
{
    public class SegmentsReader
    {
        public IEnumerable<Segment> ReadAllFrom(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return Enumerable.Empty<Segment>();
            }

            var result = new LinkedList<Segment>();
            var index = 0;

            while (index < source.Length)
            {
                if (char.IsWhiteSpace(source[index]))
                {
                    index++;
                    continue;
                }

                var segment = ExtractSegment(index, source);
                index = segment.EndOffset;

                result.AddLast(segment);
            }

            return result;
        }

        private Segment ExtractSegment(int beginOffset, string source)
        {
            var isQuoted = QuotedSegmentStrategy.IsQuote(source, beginOffset);

            var analyzer = isQuoted ?
                new QuotedSegmentStrategy() : 
                new DefaultSegmentStrategy();

            return analyzer.ExtractSegment(beginOffset, source);
        }
    }
}