using System;

namespace Clee.Tests
{
    internal class RouteBuilder
    {
        private Type _commandType;
        private string _name;

        public RouteBuilder()
        {
            _commandType = typeof (DummyCommand);
            _name = "dummy";
        }

        public RouteBuilder WithCommandType(Type commandType)
        {
            _commandType = commandType;
            return this;
        }

        public RouteBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public Route Build()
        {
            return new Route(new CommandMetaData(_commandType), _name);
        }
    }
}