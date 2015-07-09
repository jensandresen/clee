using System;
using System.Collections.Generic;
using System.Linq;

namespace Clee
{
    public class DefaultCommandRegistry : ICommandRegistry
    {
        private readonly Dictionary<string, CommandRegistration> _commandTypes = new Dictionary<string, CommandRegistration>();
        private CommandNameConvention _nameConvention = new DefaultCommandNameConvention();

        public Type Find(string commandName)
        {
            var name = _nameConvention.ExtractCommandNameFrom(commandName);

            CommandRegistration result;

            if (_commandTypes.TryGetValue(name, out result))
            {
                return result.ImplementationType;
            }

            return null;
        }

        public IEnumerable<CommandRegistration> GetAll()
        {
            return _commandTypes.Values;
        }

        public CommandRegistration Register(string commandName, Type commandType)
        {
            var alreadyContains = Contains(commandType);
            if (alreadyContains)
            {
                return null;
            }

            var isRealCommand = TypeUtils.IsAssignableToGenericType(commandType, typeof(ICommand<>));
            if (!isRealCommand)
            {
                throw new NotSupportedException(string.Format("Only types that implement {0} are allowed.", typeof(ICommand<>).FullName));
            }

            var implementedCommands = TypeUtils.ExtractCommandImplementationsFromType(commandType);
            if (implementedCommands.Length > 1)
            {
                var commandNames = string.Join(", ", implementedCommands.Select(x => x.FullName).ToArray());
                throw new NotSupportedException(string.Format("Type {0} implements more than one command which is not currently supported. It implements: {1}", commandType.FullName, commandNames));
            }

            var existingCommand = Find(commandName);
            if (existingCommand != null)
            {
                throw new Exception(string.Format("The command name \"{1}\" is already registered with command type {0}", existingCommand.FullName, commandName));
            }

            var registration = new CommandRegistration(
                commandName: commandName,
                commandType: TypeUtils.ExtractCommandImplementationsFromType(commandType).First(),
                argumentType: TypeUtils.ExtractArgumentTypesFromCommand(commandType).First(),
                implementationType: commandType
                );

            _commandTypes.Add(commandName, registration);

            return registration;
        }

        public CommandRegistration Register(Type commandType)
        {
            string commandName = null;

            var implementedCommands = TypeUtils
                .ExtractCommandImplementationsFromType(commandType)
                .FirstOrDefault();

            if (implementedCommands != null)
            {
                commandName = AttributeHelper.GetName(commandType, implementedCommands);
            }

            if (string.IsNullOrWhiteSpace(commandName))
            {
                commandName = _nameConvention.ExtractCommandNameFrom(commandType);
            }
            
            return Register(commandName, commandType);
        }

        public void Register(IEnumerable<Type> commandTypes)
        {
            foreach (var commandType in commandTypes)
            {
                Register(commandType);
            }
        }

        [Obsolete("Will be removed in v2.0.0")]
        public static string ExtractCommandNameFrom(Type commandType)
        {
            return ExtractCommandNameFrom(commandType.Name);
        }

        [Obsolete("Will be removed in v2.0.0")]
        public static string ExtractCommandNameFrom(string typeName)
        {
            return new DefaultCommandNameConvention().ExtractCommandNameFrom(typeName);
        }

        private bool Contains(Type commandType)
        {
            return GetAll().Any(x => x.ImplementationType == commandType);
        }

        public void ChangeCommandNameConvention(CommandNameConvention convention)
        {
            _nameConvention = convention;
        }
    }
}