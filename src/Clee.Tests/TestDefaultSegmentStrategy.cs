using Clee.Parsing;
using Clee.Tests.Helpers;
using Xunit;

namespace Clee.Tests
{
    public class TestDefaultSegmentStrategy
    {
        [Fact]
        public void can_extract_segment_value()
        {
            var sut = new DefaultSegmentStrategy();
            var result = sut.ExtractSegment(0, "foo");

            Assert.Equal("foo", result.Value);
        }

        [Fact]
        public void can_extract_segment_begin_offset()
        {
            var sut = new DefaultSegmentStrategy();
            var result = sut.ExtractSegment(0, "foo");

            Assert.Equal(0, result.BeginOffset);
        }

        [Fact]
        public void can_extract_segment_end_offset()
        {
            var sut = new DefaultSegmentStrategy();
            var result = sut.ExtractSegment(0, "foo");

            Assert.Equal(3, result.EndOffset);
        }

        [Fact]
        public void throws_expected_exception_when_segment_could_not_be_parsed()
        {
            var sut = new DefaultSegmentStrategy();
            Assert.Throws<SegmentException>(() => sut.ExtractSegment(0, "foo\""));
        }

        [Fact]
        public void exception_thrown_indicates_where_the_issue_offset_is_located()
        {
            var sut = new DefaultSegmentStrategy();
            var result = ExceptionHelper
                .From(() => sut.ExtractSegment(0, "foo\""))
                .Grab<SegmentException>();

            Assert.Equal(3, result.ErrorOffset);
        }

        [Fact]
        public void exception_thrown_contains_original_input_string()
        {
            var input = "foo\"";

            var sut = new DefaultSegmentStrategy();
            var result = ExceptionHelper
                .From(() => sut.ExtractSegment(0, input))
                .Grab<SegmentException>();

            Assert.Equal(input, result.Input);
        }

        [Fact]
        public void throws_exception_if_segment_starts_with_illegal_character()
        {
            var input = "\"foo";

            var sut = new DefaultSegmentStrategy();
            Assert.Throws<SegmentException>(() => sut.ExtractSegment(0, input));
        }

    }
}