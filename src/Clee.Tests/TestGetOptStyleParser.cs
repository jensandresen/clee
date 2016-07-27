using System;
using Xunit;

namespace Clee.Tests
{
    public class TestGetOptStyleParser
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void returns_expected_on_empty_input(string input)
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse(input);

            Assert.Null(result);
        }

        #region path
        
        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        [InlineData("baz")]
        [InlineData("qux")]
        public void returns_expected_path_on_simple_input(string input)
        {
            var expected = new Path(input);
            var sut = new GetOptStyleParserBuilder().Build();

            var result = sut.Parse(input);

            Assert.Equal(expected, result.Path);
        }

        [Theory]
        [InlineData("foo --someattribute", "foo")]
        [InlineData("bar --someattribute", "bar")]
        [InlineData("foo -f", "foo")]
        [InlineData("bar -b", "bar")]
        [InlineData("foo --someattribute1 --someattribute2", "foo")]
        [InlineData("bar --someattribute1 --someattribute2", "bar")]
        [InlineData("foo --someattribute -f", "foo")]
        [InlineData("bar --someattribute -b", "bar")]
        public void returns_expected_path_on_simple_input_with_arguments(string input, string expectedPath)
        {
            var sut = new GetOptStyleParserBuilder().Build();

            var result = sut.Parse(input);

            Assert.Equal(new Path(expectedPath), result.Path);
        }

        [Fact]
        public void returns_expected_path_on_input_with_path_that_has_segments()
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse("foo bar baz qux");

            var expected = new Path("foo");
            expected.AddSegment("bar");
            expected.AddSegment("baz");
            expected.AddSegment("qux");

            Assert.Equal(expected, result.Path);
        }

        [Fact]
        public void returns_expected_path_on_input_with_path_that_has_segments_and_arguments()
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse("foo bar baz --qux");

            var expected = new Path("foo");
            expected.AddSegment("bar");
            expected.AddSegment("baz");

            Assert.Equal(expected, result.Path);
        }

        [Fact]
        public void returns_expected_path_on_input_with_path_that_has_segments_and_arguments_with_values()
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse("foo bar --baz qux");

            var expected = new Path("foo");
            expected.AddSegment("bar");

            Assert.Equal(expected, result.Path);
        }

        #endregion

        #region arguments

        [Fact]
        public void returns_expected_when_no_arguments_are_specified_in_input()
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse("foo");

            Assert.Empty(result.Arguments);
        }

        [Fact]
        public void returns_expected_arguments_when_single_long_name_argument_without_value_has_been_passed()
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse("foo --bar");

            Assert.Equal(new[] {new Argument("bar", "")}, result.Arguments);
        }

        [Fact]
        public void returns_expected_arguments_when_single_short_name_argument_without_value_has_been_passed()
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse("foo -b");

            Assert.Equal(new[] {new Argument("b", "")}, result.Arguments);
        }

        [Fact]
        public void returns_expected_arguments_when_multiple_long_name_arguments_without_value_has_been_passed()
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse("foo --bar --baz --qux");

            Assert.Equal(new[]
            {
                new Argument("bar", ""),
                new Argument("baz", ""),
                new Argument("qux", ""),
            }, result.Arguments);
        }

        [Fact]
        public void returns_expected_arguments_when_single_long_name_argument_with_value_has_been_passed()
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse("foo --bar baz");

            Assert.Equal(new[] {new Argument("bar", "baz")}, result.Arguments);
        }

        [Theory]
        [InlineData("\"foo\"", "foo")]
        [InlineData("\"bar\"", "bar")]
        [InlineData("\"baz\"", "baz")]
        [InlineData("\"foo bar\"", "foo bar")]
        public void supports_quoted_argument_value(string quotedInputValue, string expectedArgumentValue)
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse(string.Format("foo --bar {0}", quotedInputValue));

            Assert.Equal(new[] { new Argument("bar", expectedArgumentValue) }, result.Arguments);
        }

        [Theory]
        [InlineData("foo\\\"bar", "foo\"bar")]
        public void unescapes_escaped_quotes_in_value(string quotedInputValue, string expectedArgumentValue)
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse(string.Format("foo --bar \"{0}\"", quotedInputValue));

            Assert.Equal(new[] { new Argument("bar", expectedArgumentValue) }, result.Arguments);
        }

        #endregion

        #region errors

        [Fact]
        public void throws_expected_exception_when_path_is_missing_from_input()
        {
            var sut = new GetOptStyleParserBuilder().Build();
            Assert.Throws<ParseException>(() => sut.Parse("--foo"));
        }

        #endregion
    }
}