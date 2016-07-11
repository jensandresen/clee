using System;

namespace Clee.Tests
{
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