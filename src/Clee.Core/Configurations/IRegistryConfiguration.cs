using System;
using System.Collections.Generic;
using System.Reflection;

namespace Clee.Configurations
{
    public interface IRegistryConfiguration
    {
        IRegistryConfiguration Use(ICommandRegistry registry);
        IRegistryConfiguration Register(Type commandType);
        IRegistryConfiguration Register(string commandName, Type commandType);
        IRegistryConfiguration Register(IEnumerable<Type> commandTypes);
        IRegistryConfiguration RegisterFromAssembly(Assembly assembly);
    }
}