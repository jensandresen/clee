using Clee.Routing;
using Moq;

namespace Clee.Tests.Builders
{
    internal class RouteEngineBuilder
    {
        private ICommandPathStrategy _commandPathStrategy;

        public RouteEngineBuilder()
        {
            _commandPathStrategy = new Mock<ICommandPathStrategy>().Object;
        }

        public RouteEngineBuilder WithCommandPathStrategy(ICommandPathStrategy commandPathStrategy)
        {
            _commandPathStrategy = commandPathStrategy;
            return this;
        }

        public RouteEngine Build()
        {
            return new RouteEngine(_commandPathStrategy);
        }
    }
}