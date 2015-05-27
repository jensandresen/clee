using System;

namespace Clee.Configurations
{
    public interface IEngineConfiguration
    {
        IEngineConfiguration Registry(Action<IRegistryConfiguration> configuration);
        IEngineConfiguration Factory(Action<IFactoryConfiguration> configuration);
        IEngineConfiguration Mapper(Action<IMapperConfiguration> configuration);
    }
}