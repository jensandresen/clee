namespace Clee.Parsing
{
    public interface ISegmentStrategy
    {
        Segment ExtractSegment(int beginOffset, string source);
    }
}