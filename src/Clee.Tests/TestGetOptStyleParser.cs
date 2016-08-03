using System;
using System.Linq;
using Clee.Parsing;
using Clee.Routing;
using Clee.Tests.Builders;
using Clee.Tests.Helpers;
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
        [InlineData("foo", "foo")]
        [InlineData("bar", "bar")]
        [InlineData("\"foo bar\"", "foo bar")]
        [InlineData("\"foo\\\"bar\"", "foo\"bar")]
        public void returns_expected_arguments_when_single_short_name_argument_with_value_has_been_passed(string inputValue, string expectedValue)
        {
            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse("foo -b " + inputValue);

            Assert.Equal(new[] {new Argument("b", expectedValue)}, result.Arguments);
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

        [Fact]
        public void supports_multi_flags()
        {
            var sut = new GetOptStyleParserBuilder().Build();

            var result = sut.Parse("cmd arg -abc");

            var expected = new[]
            {
                new Argument("a", ""),
                new Argument("b", ""),
                new Argument("c", ""),
            };

            Assert.Equal(expected, result.Arguments);
        }

        [Theory]
        [InlineData("foo", "foo")]
        [InlineData("bar", "bar")]
        [InlineData("\"foo bar\"", "foo bar")]
        [InlineData("\"foo\\\"bar\"", "foo\"bar")]
        public void multi_flags_gets_expected_value_if_value_when_present(string inputValue, string expectedValue)
        {
            var sut = new GetOptStyleParserBuilder().Build();

            var result = sut.Parse("cmd arg -abc " + inputValue);

            var expected = new[]
            {
                new Argument("a", ""),
                new Argument("b", ""),
                new Argument("c", expectedValue),
            };

            Assert.Equal(expected, result.Arguments);
        }


        #endregion

        #region errors

        [Fact]
        public void throws_expected_exception_when_path_is_missing_from_input()
        {
            var sut = new GetOptStyleParserBuilder().Build();
            Assert.Throws<ParseException>(() => sut.Parse("--foo"));
        }

        [Theory]
        [InlineData("--foo", 0)]
        [InlineData(" --foo", 1)]
        [InlineData("  --foo", 2)]
        [InlineData("-f", 0)]
        [InlineData(" -f", 1)]
        [InlineData("  -f", 2)]
        public void returns_expected_error_offset_when_path_is_missing_from_input(string input, int expectedErrorOffset)
        {
            var sut = new GetOptStyleParserBuilder().Build();
            
            var result = ExceptionHelper
                .From(() => sut.Parse(input))
                .Grab<ParseException>();

            Assert.Equal(expectedErrorOffset, result.ErrorOffset);
        }

        [Theory]
        [InlineData("cmd --argname argvalue illegal_trailing_value")]
        [InlineData("cmd --argname argvalue illegal_trailing_value another_illegal_trailing_value")]
        [InlineData("cmd --arg1name arg1value --arg2name arg2value illegal_trailing_value")]
        [InlineData("cmd --arg1name arg1value illegal_trailing_value --arg2name arg2value")]
        public void throws_exception_on_illegal_trailing_argument_value(string input)
        {
            var sut = new GetOptStyleParserBuilder().Build();
            Assert.Throws<ParseException>(() => sut.Parse(input));
        }

        [Fact]
        public void returns_expected_error_offset_on_illegal_trailing_argument_value()
        {
            var input = "cmd --argname argvalue illegal_trailing_value";

            var sut = new GetOptStyleParserBuilder().Build();

            var result = ExceptionHelper
                .From(() => sut.Parse(input))
                .Grab<ParseException>();

            Assert.Equal(
                expected: input.IndexOf("illegal_trailing_value"),
                actual: result.ErrorOffset
                );
        }

        [Theory]
        [InlineData("---foo")]
        [InlineData("----foo")]
        [InlineData("-----foo")]
        [InlineData("------foo")]
        [InlineData("-- foo")]
        [InlineData("- f")]
        public void throws_exception_on_unsupported_argument_prefix(string argumentDefinition)
        {
            var input = string.Format("cmd {0}", argumentDefinition);

            var sut = new GetOptStyleParserBuilder().Build();
            Assert.Throws<ParseException>(() => sut.Parse(input));
        }

        [Theory]
        [InlineData("cmd ---arg", 4)]
        [InlineData("cmd ----arg", 4)]
        [InlineData("cmd -- arg", 6)]
        [InlineData("cmd --  arg", 6)]
        [InlineData("cmd -  a", 5)]
        [InlineData("cmd .arg", 4)]
        [InlineData("cmd ..arg", 4)]
        [InlineData("cmd \"arg\"", 4)]
        public void returns_expected_error_offset_on_unsupported_argument_prefix(string input, int expectedOffset)
        {
            var sut = new GetOptStyleParserBuilder().Build();

            var result = ExceptionHelper
                .From(() => sut.Parse(input))
                .Grab<ParseException>();

            Assert.Equal(expectedOffset, result.ErrorOffset);
        }

        [Theory]
        [InlineData("cmd \"arg\"", 4)]
        [InlineData("\"arg\"", 0)]
        public void returns_expected_error_offset_on_unsupported_quoted_path_segment(string input, int expectedOffset)
        {
            var sut = new GetOptStyleParserBuilder().Build();

            var result = ExceptionHelper
                .From(() => sut.Parse(input))
                .Grab<ParseException>();

            Assert.Equal(expectedOffset, result.ErrorOffset);
        }

        #endregion
    }
}