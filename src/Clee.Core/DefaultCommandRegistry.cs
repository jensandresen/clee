using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Clee.Types;

namespace Clee
{
    public class DefaultCommandRegistry : ICommandRegistry
    {
        private readonly Dictionary<string, Type> _commandTypes = new Dictionary<string, Type>();

        public Type Find(string commandName)
        {
            var name = ExtractCommandNameFrom(commandName);

            Type result;
            _commandTypes.TryGetValue(name, out result);

            return result;
        }

        public IEnumerable<Type> GetAll()
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

            _commandTypes.Add(commandName, commandType);
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
            return _commandTypes.ContainsValue(commandType);
        }
    }
}