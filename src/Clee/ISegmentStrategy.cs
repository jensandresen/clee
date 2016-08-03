namespace Clee
{
    public interface ISegmentStrategy
    {
        Segment ExtractSegment(int beginOffset, string source);
    }
}