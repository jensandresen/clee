using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine("input: " + quotedInputValue);
            Console.WriteLine("expected: " + expectedArgumentValue);

            var sut = new GetOptStyleParserBuilder().Build();
            var result = sut.Parse(string.Format("foo --bar {0}", quotedInputValue));

            Assert.Equal(new[] { new Argument("bar", expectedArgumentValue) }, result.Arguments);
        }

        #endregion
    }

    public class GetOptStyleParser
    {
        public ParseResult Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            var segments = GetSegmentsFrom(input);
            var path = GetPathFrom(segments);
            var arguments = GetArgumentsFrom(segments);

            return new ParseResult(path, arguments);
        }

        private IEnumerable<Argument> GetArgumentsFrom(IEnumerable<Segment> segments)
        {
            var candidates = new Queue<Segment>(segments.SkipWhile(x => !x.Value.StartsWith("-")));

            var result = new LinkedList<Argument>();

            while (candidates.Count > 0)
            {
                var seg = candidates.Dequeue();

                var argumentName = seg.Value.Replace("-", "");
                var argumentValue = "";

                if (candidates.Count > 0)
                {
                    var nextSegment = candidates.Peek();
                    if (nextSegment != null && !nextSegment.Value.StartsWith("-"))
                    {
                        var temp = candidates.Dequeue();
                        argumentValue = temp.Value;
                        argumentValue = argumentValue.Trim('"');
                        argumentValue = argumentValue.Replace("\\\"", "\"");
                    }
                }

                result.AddLast(new Argument(
                    name: argumentName,
                    value: argumentValue
                    ));
            }

            return result;
        }

        private Path GetPathFrom(IEnumerable<Segment> segments)
        {
            var pathCandidates = segments
                .TakeWhile(x => !x.Value.StartsWith("-"))
                .ToArray();

            var path = new Path(pathCandidates[0].Value);

            foreach (var seg in pathCandidates.Skip(1))
            {
                path.AddSegment(seg.Value);
            }

            return path;
        }

        private Segment[] GetSegmentsFrom(string input)
        {
            var segmentsReader = new SegmentsReader();
            
            return segmentsReader
                .ReadAllFrom(input)
                .ToArray();
        }
    }

    public class ParseResult
    {
        public ParseResult(Path path, IEnumerable<Argument> arguments)
        {
            Path = path;
            Arguments = arguments;
        }

        public Path Path { get; private set; }
        public IEnumerable<Argument> Arguments { get; private set; }
    }

    internal class GetOptStyleParserBuilder
    {
        public GetOptStyleParser Build()
        {
            return new GetOptStyleParser();
        }
    }

    public class Argument
    {
        public Argument(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }

        #region equality members

        protected bool Equals(Argument other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Argument) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null
                    ? Name.GetHashCode()
                    : 0)*397) ^ (Value != null
                        ? Value.GetHashCode()
                        : 0);
            }
        }

        public static bool operator ==(Argument left, Argument right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Argument left, Argument right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    public class TestArgument
    {
        [Fact]
        public void returns_expected_when_comparing_two_equal_instances()
        {
            var left = new Argument("foo", "bar");
            var right = new Argument("foo", "bar");

            Assert.Equal(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_equal_instances_using_operators()
        {
            var left = new Argument("foo", "bar");
            var right = new Argument("foo", "bar");

            Assert.True(left == right);
            Assert.False(left != right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_non_equal_instances()
        {
            var left = new Argument("foo", "bar");
            var right = new Argument("baz", "qux");

            Assert.NotEqual(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_non_equal_instances_using_operators()
        {
            var left = new Argument("foo", "bar");
            var right = new Argument("baz", "qux");

            Assert.True(left != right);
            Assert.False(left == right);
        }
    }
}