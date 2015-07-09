using System;

namespace Clee.Configurations
{
    internal class EngineBuilder : IEngineConfiguration
    {
        private readonly RegistryConfiguration _registryConfiguration;
        private readonly FactoryConfiguration _factoryConfiguration;
        private readonly MapperConfiguration _mapperConfiguration;
        private Action<GeneralSettings> _settingsModifier = settings => { /* no modifications - use default settings */ };

        public EngineBuilder()
        {
            _registryConfiguration = new RegistryConfiguration();
            _factoryConfiguration = new FactoryConfiguration();
            _mapperConfiguration = new MapperConfiguration();
        }

        public IEngineConfiguration Registry(Action<IRegistryConfiguration> configuration)
        {
            configuration(_registryConfiguration);
            return this;
        }

        public IEngineConfiguration Factory(Action<IFactoryConfiguration> configuration)
        {
            configuration(_factoryConfiguration);
            return this;
        }

        public IEngineConfiguration Mapper(Action<IMapperConfiguration> configuration)
        {
            configuration(_mapperConfiguration);
            return this;
        }

        public IEngineConfiguration Settings(Action<GeneralSettings> configuration)
        {
            _settingsModifier = configuration;
            return this;
        }

        public CleeEngine Build()
        {
            var registry = _registryConfiguration.Build();
            var factory = _factoryConfiguration.Build();
            var mapper = _mapperConfiguration.Build();

            var engine = new CleeEngine(
                commandRegistry: registry,
                commandFactory: factory,
                argumentMapper: mapper,
                commandExecutor: new DefaultCommandExecutor()
                );

            _settingsModifier(engine.Settings);

            return engine;
        }
    }
}