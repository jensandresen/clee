using System;
using System.Linq;
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
        public void returns_expected_routes_when_registering_from_command_1()
        {
            var sut = new RouteEngineBuilder().Build();
            
            sut.RegisterRouteFrom<FooCommand>();

            var expected = new RouteBuilder()
                .WithCommandType(typeof (FooCommand))
                .WithName("foo")
                .Build();

            Assert.Equal(new[] {expected}, sut.Routes);
        }

        [Fact]
        public void returns_expected_routes_when_registering_from_command_2()
        {
            var sut = new RouteEngineBuilder().Build();
            
            sut.RegisterRouteFrom<BarCommand>();

            var expected = new RouteBuilder()
                .WithCommandType(typeof (BarCommand))
                .WithName("bar")
                .Build();

            Assert.Equal(new[] {expected}, sut.Routes);
        }

        [Fact]
        public void when_registering_from_command_using_generic_api_the_route_has_expected_command_type_1()
        {
            var sut = new RouteEngineBuilder().Build();

            sut.RegisterRouteFrom<FooCommand>();
            var expectedCommandType = typeof (FooCommand);

            Assert.Equal(
                expected: new[] {expectedCommandType},
                actual: sut.Routes.Select(x => x.CommandType)
                );
        }

        [Fact]
        public void when_registering_from_command_using_generic_api_the_route_has_expected_command_type_2()
        {
            var sut = new RouteEngineBuilder().Build();

            sut.RegisterRouteFrom<BarCommand>();
            var expectedCommandType = typeof (BarCommand);

            Assert.Equal(
                expected: new[] {expectedCommandType},
                actual: sut.Routes.Select(x => x.CommandType)
                );
        }

        [Theory]
        [InlineData(typeof(FooCommand))]
        [InlineData(typeof(BarCommand))]
        public void when_registering_from_command_using_non_generic_api_the_route_has_expected_command_type(Type expectedCommandType)
        {
            var sut = new RouteEngineBuilder().Build();

            sut.RegisterRouteFrom(expectedCommandType);

            Assert.Equal(
                expected: new[] {expectedCommandType},
                actual: sut.Routes.Select(x => x.CommandType)
                );
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
                    .WithName("foo")
                    .Build()
            };

            Assert.Equal(expected, sut.Routes);
        }

        #region searching for routes

        [Fact]
        public void returns_expected_when_searching_for_a_route_when_none_has_been_registered()
        {
            var sut = new RouteEngineBuilder().Build();
            var result = sut.FindRoute("foo");

            Assert.Null(result);
        }

        [Fact]
        public void returns_expected_route_when_searching_1()
        {
            var sut = new RouteEngineBuilder().Build();
            sut.RegisterRouteFrom<FooCommand>();
            
            var result = sut.FindRoute("foo");

            Assert.NotNull(result);
            Assert.Equal("foo", result.Path);
            Assert.Equal(typeof(FooCommand), result.CommandType);
        }

        [Fact]
        public void returns_expected_route_when_searching_2()
        {
            var sut = new RouteEngineBuilder().Build();
            sut.RegisterRouteFrom<BarCommand>();
            
            var result = sut.FindRoute("bar");

            Assert.NotNull(result);
            Assert.Equal("bar", result.Path);
            Assert.Equal(typeof(BarCommand), result.CommandType);
        }

        #endregion

        #region dummy classes

        private class FooCommand : DummyCommand { }
        private class BarCommand : DummyCommand { }

        #endregion
    }
}