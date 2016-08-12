using Clee.Mapping;
using Clee.Parsing;
using Clee.Routing;
using Clee.TypeResolving;
using Moq;

namespace Clee.Tests.Builders
{
    internal class CleeEngineBuilder
    {
        private ICommandResolver _commandResolver;
        private IRouteFinder _routeFinder;
        private IParser _parser;
        private IArgumentMapper _argumentMapper;

        public CleeEngineBuilder()
        {
            _commandResolver = new Mock<ICommandResolver>().Object;
            _routeFinder = new Mock<IRouteFinder>().Object;
            _parser = new Mock<IParser>().Object;
            _argumentMapper = new Mock<IArgumentMapper>().Object;
        }

        public CleeEngineBuilder WithCommandResolver(ICommandResolver commandResolver)
        {
            _commandResolver = commandResolver;
            return this;
        }

        public CleeEngineBuilder WithRouteFinder(IRouteFinder routeFinder)
        {
            _routeFinder = routeFinder;
            return this;
        }

        public CleeEngineBuilder WithParser(IParser parser)
        {
            _parser = parser;
            return this;
        }

        public CleeEngineBuilder WithArgumentMapper(IArgumentMapper argumentMapper)
        {
            _argumentMapper = argumentMapper;
            return this;
        }

        public CleeEngine Build()
        {
            return new CleeEngine(
                commandResolver: _commandResolver,
                routeFinder: _routeFinder,
                parser: _parser,
                argumentMapper: _argumentMapper
                );
        }
    }
}