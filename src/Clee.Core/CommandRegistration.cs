using System;

namespace Clee
{
    public class CommandRegistration
    {
        public CommandRegistration(string commandName, Type commandType, Type argumentType, Type implementationType)
        {
            CommandName = commandName;
            CommandType = commandType;
            ArgumentType = argumentType;
            ImplementationType = implementationType;
        }

        public string CommandName { get; private set; }
        public Type CommandType { get; private set; }
        public Type ArgumentType { get; private set; }
        public Type ImplementationType { get; private set; }
    }
}