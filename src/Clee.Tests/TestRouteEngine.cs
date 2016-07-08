using System;
using System.Collections.Generic;
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

    public class RouteEngine
    {
        private readonly ISet<Route> _routes = new HashSet<Route>();

        public IEnumerable<Route> Routes
        {
            get { return _routes; }
        }

        public void RegisterRoute(Route route)
        {
            _routes.Remove(route);
            _routes.Add(route);
        }

        public void RegisterRouteFrom<T>() where T : Command
        {
            var commandType = typeof (T);
            RegisterRoute(new Route(commandType));
        }
    }

    internal class RouteEngineBuilder
    {
        public RouteEngine Build()
        {
            return new RouteEngine();
        }
    }

    public class Route
    {
        private readonly Type _commandType;

        public Route(Type commandType)
        {
            _commandType = commandType;
        }

        public Type CommandType
        {
            get { return _commandType; }
        }

        #region equality functionality

        protected bool Equals(Route other)
        {
            return Equals(_commandType, other._commandType);
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
            return Equals((Route) obj);
        }

        public override int GetHashCode()
        {
            return (_commandType != null
                ? _commandType.GetHashCode()
                : 0);
        }

        public static bool operator ==(Route left, Route right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Route left, Route right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    internal class RouteBuilder
    {
        private Type _commandType;

        public RouteBuilder()
        {
            _commandType = typeof (DummyCommand);
        }

        public RouteBuilder WithCommandType(Type commandType)
        {
            _commandType = commandType;
            return this;
        }

        public Route Build()
        {
            return new Route(_commandType);
        }
    }
}