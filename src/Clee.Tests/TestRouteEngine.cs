using Xunit;

namespace Clee.Tests
{
    public class TestRouteEngine
    {
        [Fact]
        public void routes_are_empty_by_default()
        {
            var sut = new RouteEngineBuilder().Build();
            Assert.Empty(sut.Routes);
        }

        [Fact]
        public void returns_expected_routes_after_registering_a_single_route()
        {
            var dummyRoute = new RouteBuilder().Build();
            var sut = new RouteEngineBuilder().Build();

            sut.RegisterRoute(dummyRoute);

            Assert.Equal(new[] {dummyRoute}, sut.Routes);
        }

        [Fact]
        public void returns_expected_routes_after_registering_multiple_routes()
        {
            var dummyRoute1 = new RouteBuilder()
                .WithCommandType(typeof(FooCommand))
                .Build();

            var dummyRoute2 = new RouteBuilder()
                .WithCommandType(typeof(BarCommand))
                .Build();

            var sut = new RouteEngineBuilder().Build();

            sut.RegisterRoute(dummyRoute1);
            sut.RegisterRoute(dummyRoute2);

            Assert.Equal(new[] {dummyRoute1, dummyRoute2}, sut.Routes);
        }

        [Fact]
        public void returns_expected_routes_when_registering_from_command()
        {
            var sut = new RouteEngineBuilder().Build();
            sut.RegisterRouteFrom<FooCommand>();

            var expected = new[]
            {
                new RouteBuilder()
                    .WithCommandType(typeof (FooCommand))
                    .Build()
            };

            Assert.Equal(expected, sut.Routes);
        }

        [Fact]
        public void registering_from_a_command_is_idempotent()
        {
            var sut = new RouteEngineBuilder().Build();
            sut.RegisterRouteFrom<FooCommand>();
            sut.RegisterRouteFrom<FooCommand>();

            var expected = new[]
            {
                new RouteBuilder()
                    .WithCommandType(typeof (FooCommand))
                    .Build()
            };

            Assert.Equal(expected, sut.Routes);
        }



        #region dummy classes

        private class FooCommand : DummyCommand { }
        private class BarCommand : DummyCommand { }

        #endregion
 
    }
}