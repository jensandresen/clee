using Xunit;

namespace Clee.Tests
{
    public class TestArgument
    {
        [Fact]
        public void returns_expected_when_comparing_two_equal_instances()
        {
            var left = new Argument("foo", "bar", false);
            var right = new Argument("foo", "bar", false);

            Assert.Equal(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_equal_instances_using_operators()
        {
            var left = new Argument("foo", "bar", false);
            var right = new Argument("foo", "bar", false);

            Assert.True(left == right);
            Assert.False(left != right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_non_equal_instances()
        {
            var left = new Argument("foo", "bar", false);
            var right = new Argument("baz", "qux", false);

            Assert.NotEqual(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_non_equal_instances_using_operators()
        {
            var left = new Argument("foo", "bar", false);
            var right = new Argument("baz", "qux", false);

            Assert.True(left != right);
            Assert.False(left == right);
        }
    }
}