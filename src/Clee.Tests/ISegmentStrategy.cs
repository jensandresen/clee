namespace Clee.Tests
{
    public interface ISegmentStrategy
    {
        Segment ExtractSegment(int beginOffset, string source);
    }
}