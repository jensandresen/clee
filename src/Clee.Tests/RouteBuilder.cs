using System;

namespace Clee.Tests
{
    internal class RouteBuilder
    {
        private Type _commandType;
        private string _path;

        public RouteBuilder()
        {
            _commandType = typeof (DummyCommand);
            _path = "dummy";
        }

        public RouteBuilder WithCommandType(Type commandType)
        {
            _commandType = commandType;
            return this;
        }

        public RouteBuilder WithPath(string path)
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