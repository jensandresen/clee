using System.Linq;
using Clee.Parsing;
using Xunit;

namespace Clee.Tests
{
    public class TestCommandLineParser
    {
        [Theory]
        [InlineData("foo", "foo")]
        [InlineData("foo bar", "foo")]
        [InlineData("bar foo", "bar")]
        public void returns_expected_command_name_from_input(string input, string expected)
        {
            var result = CommandLineParser.ExtractCommandNameFrom(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("foo ")]
        public void returns_expected_when_command_arguments_are_not_available_in_the_input(string input)
        {
            var result = CommandLineParser.ExtractArgumentsFrom(input);
            Assert.Empty(result);
        }

        [Fact]
        public void testname()
        {
            var expected = new[]
            {
                new Argument("foo", "1"),
                new Argument("bar", "2"),
            };

            var result = CommandLineParser.ExtractArgumentsFrom("cmd -foo 1 -bar 2");
            
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("-foo bar", "foo", "bar")]
        [InlineData("-foo  bar", "foo", "bar")]
        [InlineData("-foo   bar", "foo", "bar")]
        [InlineData("-foo bar ", "foo", "bar")]
        [InlineData("-foo bar  ", "foo", "bar")]
        [InlineData(" -foo bar", "foo", "bar")]
        [InlineData("  -foo bar", "foo", "bar")]
        [InlineData("-foo bar-baz", "foo", "bar-baz")]
        [InlineData("-foo bar-baz-qux", "foo", "bar-baz-qux")]
        public void can_parse_single_argument(string input, string expectedName, string expectedValue)
        {
            var result = CommandLineParser.ParseArguments(input);
            
            var expected = new[]
            {
                new Argument(expectedName, expectedValue),
            };

            Assert.Equal(expected, result);
        }

        [Fact]
        public void can_parse_two_arguments()
        {
            var input = "-foo bar -baz qux";
            var expected = new[]
            {
                new Argument("foo", "bar"),
                new Argument("baz", "qux"),
            };

            var result = CommandLineParser.ParseArguments(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("-foo \"bar\"", "foo", "bar")]
        [InlineData("-foo \"bar baz\"", "foo", "bar baz")]
        [InlineData("-foo \"bar baz qux\"", "foo", "bar baz qux")]
        [InlineData("-foo \"bar \"", "foo", "bar ")]
        [InlineData("-foo \"bar   \"", "foo", "bar   ")]
        [InlineData("-foo \"bar-baz\"", "foo", "bar-baz")]
        [InlineData("-foo \"bar-baz-qux\"", "foo", "bar-baz-qux")]
        [InlineData("-foo \"-bar-\"", "foo", "-bar-")]
        [InlineData("-foo \"-bar\"", "foo", "-bar")]
        [InlineData("-foo \"bar-\"", "foo", "bar-")]
        public void can_parse_single_argument_with_quoted_value(string input, string expectedName, string expectedValue)
        {
            var result = CommandLineParser.ParseArguments(input);

            var expected = new[]
            {
                new Argument(expectedName, expectedValue),
            };

            Assert.Equal(expected, result);
        }

        [Fact]
        public void can_parse_multiple_arguments_with_quoted_value()
        {
            var input = "-foo \"bar\" -baz \"qux\"";
            var expected = new[]
            {
                new Argument("foo", "bar"),
                new Argument("baz", "qux"),
            };

            var result = CommandLineParser.ParseArguments(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("-foo", "foo")]
        [InlineData("-foo ", "foo")]
        public void can_parse_argument_with_no_value(string input, string expectedName)
        {
            var result = CommandLineParser.ParseArguments(input).ToArray();

            var expected = new[]
            {
                new Argument(expectedName, ""),
            };

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("-foo -bar", "foo,bar")]
        public void can_parse_multiple_arguments_with_no_value(string input, string expectedNames)
        {
            var result = CommandLineParser.ParseArguments(input).ToArray();

            var expected = expectedNames
                .Split(',')
                .Select(name => new Argument(name, ""))
                .ToArray();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("-foo bar -baz \"qux\"")]
        [InlineData("-foo \"bar\" -baz qux")]
        public void placement_of_quotes(string input)
        {
            var result = CommandLineParser.ParseArguments(input).ToArray();

            var expected = new[]
            {
                new Argument("foo", "bar"),
                new Argument("baz", "qux"),
            };

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("foo", "foo")]
        [InlineData(" foo", "foo")]
        [InlineData("  foo", "foo")]
        [InlineData("                   foo", "foo")]
        [InlineData("foo bar", "foo,bar")]
        [InlineData("foo  bar", "foo,bar")]
        [InlineData("foo   bar", "foo,bar")]
        [InlineData("foo                   bar", "foo,bar")]
        [InlineData("foo bar baz", "foo,bar,baz")]
        [InlineData("foo bar baz qux", "foo,bar,baz,qux")]
        [InlineData("\"foo\"", "\"foo\"")]
        [InlineData("foo \"bar baz qux\"", "foo,\"bar baz qux\"")]
        [InlineData("foo \"bar\" \"baz\" \"qux\"", "foo,\"bar\",\"baz\",\"qux\"")]
        [InlineData("foo ", "foo")]
        [InlineData("foo  ", "foo")]
        [InlineData("foo          ", "foo")]
        [InlineData("foo bar ", "foo,bar")]
        [InlineData("foo bar            ", "foo,bar")]
        public void can_sprint_input_into_sections(string input, string expectedSections)
        {
            var result = CommandLineParser.SplitArgumentsIntoSections(input).ToArray();
            var expected = expectedSections.Split(',');

            Assert.Equal(expected, result);
        }
    }
}