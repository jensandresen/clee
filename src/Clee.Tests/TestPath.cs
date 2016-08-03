using System;
using Clee.Routing;
using Xunit;

namespace Clee.Tests
{
    public class TestPath
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("          ")]
        [InlineData("foo bar")]
        public void ctor_throws_exception_if_input_is_invalid(string input)
        {
            Assert.Throws<ArgumentException>(() => new Path(input));
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        [InlineData("baz")]
        [InlineData("qux")]
        public void to_string_returns_expected_when_initialized_with_single_value(string value)
        {
            var sut = new Path(value);

            Assert.Equal("/" + value, sut.ToString());
        }

        [Fact]
        public void to_string_returns_expected_when_single_segment_are_added_to_the_path()
        {
            var sut = new Path("foo");
            sut.AddSegment("bar");

            Assert.Equal("/foo/bar", sut.ToString());
        }

        [Fact]
        public void to_string_returns_expected_when_multiple_segments_are_added_to_the_path()
        {
            var sut = new Path("foo");
            sut.AddSegment("bar");
            sut.AddSegment("baz");
            sut.AddSegment("qux");

            Assert.Equal("/foo/bar/baz/qux", sut.ToString());
        }

        [Fact]
        public void returns_expected_when_comparing_two_equal_instances()
        {
            var left = new Path("foo");
            var right = new Path("foo");

            Assert.Equal(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_equal_instances_with_segments()
        {
            var left = new Path("foo");
            left.AddSegment("bar");

            var right = new Path("foo");
            right.AddSegment("bar");

            Assert.Equal(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_equal_instances_using_operators()
        {
            var left = new Path("foo");
            var right = new Path("foo");

            Assert.True(left == right);
            Assert.False(left != right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_equal_instances_with_segments_using_operators()
        {
            var left = new Path("foo");
            left.AddSegment("bar");

            var right = new Path("foo");
            right.AddSegment("bar");

            Assert.True(left == right);
            Assert.False(left != right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_non_equal_instances()
        {
            var left = new Path("foo");
            var right = new Path("bar");

            Assert.NotEqual(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_non_equal_instances_using_operators()
        {
            var left = new Path("foo");
            var right = new Path("bar");

            Assert.True(left != right);
            Assert.False(left == right);
        }

        [Theory]
        [InlineData("foo", "/foo")]
        [InlineData("/foo", "/foo")]
        public void returns_expected_when_parsing_valid_simple_input(string input, string expected)
        {
            var result = Path.Parse(input);
            Assert.Equal(expected, result.ToString());
        }

        [Theory]
        [InlineData("foo bar", "/foo/bar")]
        [InlineData("/foo/bar", "/foo/bar")]
        [InlineData("foo  bar", "/foo/bar")]
        [InlineData("foo   bar", "/foo/bar")]
        [InlineData("foo               bar", "/foo/bar")]
        [InlineData("/foo//bar", "/foo/bar")]
        [InlineData("/foo///bar", "/foo/bar")]
        [InlineData("//foo/bar", "/foo/bar")]
        [InlineData("///foo/bar", "/foo/bar")]
        [InlineData("/foo/bar/", "/foo/bar")]
        [InlineData(" / foo / bar ", "/foo/bar")]
        public void returns_expected_when_parsing_valid_input_with_segments(string input, string expected)
        {
            var result = Path.Parse(input);
            Assert.Equal(expected, result.ToString());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("/")]
        [InlineData(" /")]
        [InlineData(" / ")]
        public void throws_exception_when_parsing_invalid_input(string invalidInput)
        {
            Assert.Throws<ArgumentException>(() => Path.Parse(invalidInput));
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("/foo")]
        [InlineData("foo bar")]
        [InlineData("/foo/bar")]
        public void try_parse_returns_expected_when_parsing_valid_input(string input)
        {
            Path notUsedInstance;
            var result = Path.TryParse(input, out notUsedInstance);

            Assert.True(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("/")]
        public void try_parse_returns_expected_when_parsing_invalid_input(string input)
        {
            Path notUsedInstance;
            var result = Path.TryParse(input, out notUsedInstance);

            Assert.False(result);
        }

        [Theory]
        [InlineData("foo", "/foo")]
        [InlineData("foo bar", "/foo/bar")]
        public void try_parse_returns_expected_path_instance_when_parsing_valid_input(string input, string expected)
        {
            Path instance;
            Path.TryParse(input, out instance);

            Assert.NotNull(instance);
            Assert.Equal(expected, instance.ToString());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("/")]
        public void try_parse_returns_expected_path_instance_when_parsing_invalid_input(string input)
        {
            Path instance;
            Path.TryParse(input, out instance);

            Assert.Null(instance);
        }
    }
}