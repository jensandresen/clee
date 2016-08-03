using System.Globalization;
using System.Linq;
using Clee.Parsing;
using Clee.Tests.Builders;
using Clee.Tests.Helpers;
using Xunit;

namespace Clee.Tests
{
    public class TestSegmentsReader
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("               ")]
        public void returns_expected_on_no_or_empty_input(string input)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut.ReadAllFrom(input);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #region single segment

        [Theory]
        [InlineData("foo", "foo")]
        [InlineData(" foo", "foo")]
        [InlineData("  foo", "foo")]
        [InlineData("          foo", "foo")]
        [InlineData("foo ", "foo")]
        [InlineData("foo  ", "foo")]
        [InlineData("foo   ", "foo")]
        [InlineData("foo          ", "foo")]
        [InlineData("  foo  ", "foo")]
        [InlineData("--foo", "--foo")]
        [InlineData("-f", "-f")]
        public void single_segment__returns_expected_value(string input, string expected)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut.ReadAllFrom(input).Single();

            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData("foo", 0)]
        [InlineData(" foo", 1)]
        [InlineData("  foo", 2)]
        [InlineData("   foo", 3)]
        [InlineData("\"foo\"", 0)]
        [InlineData(" \"foo\"", 1)]
        [InlineData("  \"foo\"", 2)]
        [InlineData("   \"foo\"", 3)]
        public void single_segment__returns_expected_begin_offset(string input, int expectedBeginOffset)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut.ReadAllFrom(input).Single();

            Assert.Equal(expectedBeginOffset, result.BeginOffset);
        }

        [Theory]
        [InlineData("foo", 3)]
        [InlineData(" foo", 4)]
        [InlineData("  foo", 5)]
        [InlineData("   foo", 6)]
        [InlineData("foo ", 3)]
        [InlineData("foo  ", 3)]
        [InlineData("foo   ", 3)]
        [InlineData("foo               ", 3)]
        [InlineData("\"foo\"", 5)]
        [InlineData(" \"foo\"", 6)]
        [InlineData("  \"foo\"", 7)]
        [InlineData("   \"foo\"", 8)]
        [InlineData("\"foo\" ", 5)]
        [InlineData("\"foo\"  ", 5)]
        [InlineData("\"foo\"   ", 5)]
        [InlineData("\"foo\"           ", 5)]
        public void single_segment__returns_expected_end_offset(string input, int expectedEndOffset)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut.ReadAllFrom(input).Single();

            Assert.Equal(expectedEndOffset, result.EndOffset);
        }

        [Theory]
        [InlineData("\"foo\"", "\"foo\"")]
        [InlineData("\"foo bar\"", "\"foo bar\"")]
        public void supports_quoted_segment(string input, string expecteValue)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut
                .ReadAllFrom(input)
                .Select(x => x.Value)
                .ToArray();

            Assert.Equal(new[] {expecteValue}, result);
        }

        [Theory]
        [InlineData("\"\"", "\"\"")]
        [InlineData("\" \"", "\" \"")]
        public void supports_empty_quoted_segment(string input, string expecteValue)
        {
            var sut = new SegmentReaderBuilder().Build();

            var result = sut
                .ReadAllFrom(input)
                .Select(x => x.Value)
                .ToArray();

            Assert.Equal(new[] {expecteValue}, result);
        }

        [Theory]
        [InlineData("\"foo\\\"bar\"", "\"foo\\\"bar\"")]
        [InlineData("\"foo \\\" bar\"", "\"foo \\\" bar\"")]
        [InlineData("\"foo\\\"\"", "\"foo\\\"\"")]
        [InlineData("\"foo\\\" \"", "\"foo\\\" \"")]
        [InlineData("\"\\\"bar\"", "\"\\\"bar\"")]
        [InlineData("\" \\\"bar\"", "\" \\\"bar\"")]
        public void supports_quoted_segments_containing_escaped_quotes(string input, string expecteValue)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut
                .ReadAllFrom(input)
                .Select(x => x.Value)
                .ToArray();

            Assert.Equal(new[] {expecteValue}, result);
        }

        #endregion

        #region two segments

        [Theory]
        [InlineData("foo bar", "foo,bar")]
        [InlineData(" foo bar", "foo,bar")]
        [InlineData("  foo bar", "foo,bar")]
        [InlineData("foo bar ", "foo,bar")]
        [InlineData("foo bar  ", "foo,bar")]
        [InlineData("foo  bar", "foo,bar")]
        [InlineData("foo   bar", "foo,bar")]
        [InlineData("foo       bar", "foo,bar")]
        [InlineData("  foo  bar  ", "foo,bar")]
        public void returns_expected_segment_value_on_two_segments(string input, string expectedCsv)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut
                .ReadAllFrom(input)
                .Select(x => x.Value)
                .ToArray();

            Assert.Equal(expectedCsv.Split(','), result);
        }

        [Theory]
        [InlineData("foo bar", 4)]
        [InlineData("foo  bar", 5)]
        [InlineData("foo   bar", 6)]
        [InlineData("\"foo\" bar", 6)]
        [InlineData("foo \"bar\"", 4)]
        [InlineData("\"foo\" \"bar\"", 6)]
        public void returns_expected_begin_offset_on_second_segment(string input, int expectedBeginOffset)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut
                .ReadAllFrom(input)
                .ToArray();

            Assert.Equal(expectedBeginOffset, result[1].BeginOffset);
        }

        #endregion

        #region multiple segments

        [Theory]
        [InlineData("foo bar", "foo,bar")]
        [InlineData("foo bar baz", "foo,bar,baz")]
        [InlineData("foo bar baz qux", "foo,bar,baz,qux")]
        [InlineData("1", "1")]
        [InlineData("1 2", "1,2")]
        [InlineData("1 2 3", "1,2,3")]
        [InlineData("1 2 3 4", "1,2,3,4")]
        [InlineData("1 2 3 4 5", "1,2,3,4,5")]
        public void returns_expected_segment_value_on_multiple_segments(string input, string expectedCsv)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut
                .ReadAllFrom(input)
                .Select(x => x.Value)
                .ToArray();

            Assert.Equal(expectedCsv.Split(','), result);
        }

        [Theory]
        [InlineData("foo bar", "0,4")]
        [InlineData("foo bar baz", "0,4,8")]
        [InlineData("foo bar baz qux", "0,4,8,12")]
        public void returns_expected_segment_begin_offset_on_multiple_segments(string input, string expectedCsv)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut
                .ReadAllFrom(input)
                .Select(x => x.BeginOffset)
                .ToArray();

            Assert.Equal(expectedCsv, string.Join(",", result));
        }

        [Theory]
        [InlineData("\"f\" \"o\" \"o\"", "\"f\",\"o\",\"o\"")]
        public void supports_multiple_quoted_segments(string input, string expecteCsv)
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = sut
                .ReadAllFrom(input)
                .Select(x => x.Value)
                .ToArray();

            Assert.Equal(expecteCsv, string.Join(",", result));
        }

        #endregion

        #region errors

        [Fact]
        public void throws_expected_exception_when_quoted_segment_is_missing_ending_quote()
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = ExceptionHelper
                .From(() => sut.ReadAllFrom("foo \"bar"))
                .Grab<SegmentException>();

            Assert.NotNull(result);
            Assert.Equal(8, result.ErrorOffset);
        }

        [Fact]
        public void throws_expected_exception_when_quoted_segment_is_missing_beginning_quote()
        {
            var sut = new SegmentReaderBuilder().Build();
            var result = ExceptionHelper
                .From(() => sut.ReadAllFrom("foo bar\""))
                .Grab<SegmentException>();

            Assert.NotNull(result);
            Assert.Equal(7, result.ErrorOffset);
        }

        #endregion
    }
}