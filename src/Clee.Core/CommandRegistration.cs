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

        protected bool Equals(CommandRegistration other)
        {
            return string.Equals(CommandName, other.CommandName) && Equals(CommandType, other.CommandType) &&
                   Equals(ArgumentType, other.ArgumentType) && Equals(ImplementationType, other.ImplementationType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((CommandRegistration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (CommandName != null
                    ? CommandName.GetHashCode()
                    : 0);
                hashCode = (hashCode*397) ^ (CommandType != null
                    ? CommandType.GetHashCode()
                    : 0);
                hashCode = (hashCode*397) ^ (ArgumentType != null
                    ? ArgumentType.GetHashCode()
                    : 0);
                hashCode = (hashCode*397) ^ (ImplementationType != null
                    ? ImplementationType.GetHashCode()
                    : 0);
                return hashCode;
            }
        }

        public static bool operator ==(CommandRegistration left, CommandRegistration right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CommandRegistration left, CommandRegistration right)
        {
            return !Equals(left, right);
        }
    }
}