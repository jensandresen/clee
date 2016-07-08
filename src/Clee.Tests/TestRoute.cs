using Xunit;

namespace Clee.Tests
{
    public class TestRoute
    {
        [Fact]
        public void command_type_returns_expected()
        {
            var sut = new Route(typeof(DummyCommand));
            Assert.Equal(typeof(DummyCommand), sut.CommandType);
        }

        [Fact]
        public void returns_expected_when_comparing_two_equal_routes()
        {
            var left = new RouteBuilder().Build();
            var right = new RouteBuilder().Build();

            Assert.Equal(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_equal_routes_using_operator()
        {
            var left = new RouteBuilder().Build();
            var right = new RouteBuilder().Build();

            Assert.True(left == right);
            Assert.False(left != right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_non_equal_routes()
        {
            var left = new RouteBuilder()
                .WithCommandType(typeof(FooCommand))
                .Build();

            var right = new RouteBuilder()
                .WithCommandType(typeof(BarCommand))
                .Build();

            Assert.NotEqual(left, right);
        }

        [Fact]
        public void returns_expected_when_comparing_two_non_equal_routes_using_operator()
        {
            var left = new RouteBuilder()
                .WithCommandType(typeof(FooCommand))
                .Build();

            var right = new RouteBuilder()
                .WithCommandType(typeof(BarCommand))
                .Build();

            Assert.True(left != right);
            Assert.False(left == right);
        }

        #region dummy classes

        private abstract class BaseDummyCommand : Command
        {
            public override void Execute()
            {
                
            }
        }

        private class FooCommand : BaseDummyCommand { }
        private class BarCommand : BaseDummyCommand { }

        #endregion

    }
}