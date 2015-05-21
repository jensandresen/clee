using Clee.Types;

namespace Clee
{
    public interface IEngineConfiguration
    {
        EngineBuilder WithRegistry(ICommandRegistry registry);
        EngineBuilder WithFactory(ICommandFactory factory);
        EngineBuilder WithMapper(IArgumentMapper mapper);
        EngineBuilder WithExecutor(ICommandExecutor executor);
    }

    public class EngineBuilder : IEngineConfiguration
    {
        private ICommandRegistry _registry;
        private ICommandFactory _commandFactory;
        private IArgumentMapper _mapper;
        private ICommandExecutor _commandExecutor;

        public EngineBuilder()
        {
            _registry = new DefaultCommandRegistry();
            _commandFactory = new DefaultCommandFactory();
            _mapper = new DefaultArgumentMapper();
            _commandExecutor = new DefaultCommandExecutor();
        }

        public EngineBuilder WithRegistry(ICommandRegistry registry)
        {
            _registry = registry;
            return this;
        }

        public EngineBuilder WithFactory(ICommandFactory factory)
        {
            _commandFactory = factory;
            return this;
        }

        public EngineBuilder WithMapper(IArgumentMapper mapper)
        {
            _mapper = mapper;
            return this;
        }

        public EngineBuilder WithExecutor(ICommandExecutor executor)
        {
            _commandExecutor = executor;
            return this;
        }

        public Engine Build()
        {
            return new Engine(
                commandRegistry: _registry,
                commandFactory: _commandFactory,
                argumentMapper: _mapper,
                commandExecutor: _commandExecutor
                );
        }
    }
}