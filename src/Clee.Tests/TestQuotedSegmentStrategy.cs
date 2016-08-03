using Clee.Tests.Helpers;
using Xunit;

namespace Clee.Tests
{
    public class TestQuotedSegmentStrategy
    {
        [Fact]
        public void can_extract_quoted_segment_value()
        {
            var sut = new QuotedSegmentStrategy();
            var result = sut.ExtractSegment(0, "\"foo\"");

            Assert.Equal("\"foo\"", result.Value);
        }

        [Fact]
        public void can_extract_quoted_segment_begin_offset()
        {
            var sut = new QuotedSegmentStrategy();
            var result = sut.ExtractSegment(0, "\"foo\"");

            Assert.Equal(0, result.BeginOffset);
        }

        [Fact]
        public void can_extract_quoted_segment_end_offset()
        {
            var sut = new QuotedSegmentStrategy();
            var result = sut.ExtractSegment(0, "\"foo\"");

            Assert.Equal(5, result.EndOffset);
        }

        [Theory]
        [InlineData("\"foo")]
        [InlineData("\"foo bar")]
        [InlineData("\"foo --bar")]
        [InlineData("\"foo -b")]
        public void throws_expected_exception_when_segment_end_is_missing(string input)
        {
            var sut = new QuotedSegmentStrategy();
            Assert.Throws<SegmentException>(() => sut.ExtractSegment(0, input));
        }

        [Fact]
        public void exception_thrown_indicates_where_the_issue_offset_is_located()
        {
            var sut = new QuotedSegmentStrategy();
            var result = ExceptionHelper
                .From(() => sut.ExtractSegment(0, "\"foo"))
                .Grab<SegmentException>();

            Assert.Equal(4, result.ErrorOffset);
        }

        [Fact]
        public void exception_thrown_contains_original_input_string()
        {
            var input = "\"foo";

            var sut = new QuotedSegmentStrategy();
            var result = ExceptionHelper
                .From(() => sut.ExtractSegment(0, input))
                .Grab<SegmentException>();

            Assert.Equal(input, result.Input);
        }

        [Fact]
        public void throws_exception_if_segment_starts_with_illegal_character()
        {
            var input = "foo\"";

            var sut = new QuotedSegmentStrategy();
            Assert.Throws<SegmentException>(() => sut.ExtractSegment(0, input));
        }
    }
}