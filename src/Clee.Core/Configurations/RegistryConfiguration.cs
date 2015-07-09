using System;
using System.Collections.Generic;
using System.Reflection;

namespace Clee.Configurations
{
    internal class RegistryConfiguration : IRegistryConfiguration
    {
        private readonly List<Action<ICommandRegistry>> _modifiers = new List<Action<ICommandRegistry>>();
        private ICommandRegistry _registry;

        public RegistryConfiguration()
        {
            _registry = new DefaultCommandRegistry();
        }

        public IRegistryConfiguration Use(ICommandRegistry registry)
        {
            _registry = registry;
            return this;
        }

        public IRegistryConfiguration Register(Type commandType)
        {
            _modifiers.Add(r => r.Register(commandType));
            return this;
        }

        public IRegistryConfiguration Register(string commandName, Type commandType)
        {
            _modifiers.Add(r => r.Register(commandName, commandType));
            return this;
        }

        public IRegistryConfiguration Register(IEnumerable<Type> commandTypes)
        {
            _modifiers.Add(r => r.Register(commandTypes));
            return this;
        }

        public IRegistryConfiguration RegisterFromAssembly(Assembly assembly)
        {
            _modifiers.Add(r =>
            {
                var commandTypes = new CommandScanner().Scan(assembly);
                r.Register(commandTypes);
            });

            return this;
        }

        public IRegistryConfiguration NameConvention(CommandNameConvention convention)
        {
            _modifiers.Add(r =>
            {
                r.ChangeCommandNameConvention(convention);
            });

            return this;
        }

        public ICommandRegistry Build()
        {
            foreach (var modify in _modifiers)
            {
                modify(_registry);
            }

            return _registry;
        }
    }
}