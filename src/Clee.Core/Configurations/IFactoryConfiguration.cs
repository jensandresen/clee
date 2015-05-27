using Clee.Types;

namespace Clee.Configurations
{
    public interface IFactoryConfiguration
    {
        IFactoryConfiguration Use(ICommandFactory factory);
    }
}