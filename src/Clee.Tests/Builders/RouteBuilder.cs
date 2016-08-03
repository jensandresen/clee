using System;
using Clee.Tests.TestDoubles;

namespace Clee.Tests.Builders
{
    internal class RouteBuilder
    {
        private Type _commandType;
        private Path _path;

        public RouteBuilder()
        {
            _commandType = typeof (DummyCommand);
            _path = new Path("dummy");
        }

        public RouteBuilder WithCommandType(Type commandType)
        {
            _commandType = commandType;
            return this;
        }

        public RouteBuilder WithPath(Path path)
        {
            _path = path;
            return this;
        }

        public Route Build()
        {
            return new Route(new CommandMetaData(_commandType), _path);
        }
    }
}