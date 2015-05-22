using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Clee.Types;

namespace Clee
{
    public class DefaultCommandRegistry : ICommandRegistry
    {
        private readonly Dictionary<string, CommandRegistration> _commandTypes = new Dictionary<string, CommandRegistration>();

        public Type Find(string commandName)
        {
            var name = ExtractCommandNameFrom(commandName);

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

        public void Register(Type commandType)
        {
            var alreadyContains = Contains(commandType);
            if (alreadyContains)
            {
                return;
            }

            var isRealCommand = TypeUtils.IsAssignableToGenericType(commandType, typeof(ICommand<>));
            if (!isRealCommand)
            {
                throw new NotSupportedException(string.Format("Only types that implement {0} are allowed.", typeof(ICommand<>).FullName));
            }

            var commandName = ExtractCommandNameFrom(commandType);

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
        }

        public void Register(IEnumerable<Type> commandTypes)
        {
            foreach (var commandType in commandTypes)
            {
                Register(commandType);
            }
        }

        public static string ExtractCommandNameFrom(Type commandType)
        {
            return ExtractCommandNameFrom(commandType.Name);
        }

        public static string ExtractCommandNameFrom(string typeName)
        {
            return Regex
                .Replace(typeName, @"^(?<name>.*?)(Command|Cmd)$", "${name}", RegexOptions.IgnoreCase)
                .ToLowerInvariant();
        }

        private bool Contains(Type commandType)
        {
            return GetAll().Any(x => x.ImplementationType == commandType);
        }
    }
}