using Moq;

namespace Clee.Tests.Builders
{
    internal class CleeEngineBuilder
    {
        private ICommandResolver _commandResolver;

        public CleeEngineBuilder()
        {
            _commandResolver = new Mock<ICommandResolver>().Object;
        }

        public CleeEngineBuilder WithCommandResolver(ICommandResolver commandResolver)
        {
            _commandResolver = commandResolver;
            return this;
        }

        public CleeEngine Build()
        {
            return new CleeEngine(
                commandResolver: _commandResolver
                );
        }
    }
}