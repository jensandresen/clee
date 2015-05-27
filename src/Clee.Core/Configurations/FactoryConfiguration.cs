using Clee.Types;

namespace Clee.Configurations
{
    internal class FactoryConfiguration : IFactoryConfiguration
    {
        private ICommandFactory _factory;

        public FactoryConfiguration()
        {
            _factory = new DefaultCommandFactory();
        }

        public IFactoryConfiguration Use(ICommandFactory factory)
        {
            _factory = factory;
            return this;
        }

        public ICommandFactory Build()
        {
            return _factory;
        }
    }
}