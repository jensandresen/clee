using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clee
{
    public class CommandMetaData
    {
        private readonly Type _commandType;

        public CommandMetaData(Type commandType)
        {
            if (commandType == null)
            {
                throw new ArgumentNullException("commandType");
            }

            if (!typeof(Command).IsAssignableFrom(commandType))
            {
                throw new ArgumentException(
                    message: string.Format("The type \"{0}\" does not derive from {1}.", commandType, typeof(Command)),
                    paramName: "commandType"
                    );
            }

            _commandType = commandType;
        }

        public Type CommandType
        {
            get { return _commandType; }
        }

        public string CommandName
        {
            get
            {
                var attribute = _commandType.GetCustomAttribute<CommandAttribute>();
                if (attribute != null)
                {
                    return attribute.Name;
                }

                return _commandType
                    .Name
                    .Replace("Command", "")
                    .ToLower();                
            }
        }

        public string CommandDescription
        {
            get
            {
                var attribute = _commandType.GetCustomAttribute<CommandAttribute>();
                
                if (attribute != null)
                {
                    return attribute.Description;
                }

                return "";
            }
        }

        public IEnumerable<ArgumentMetaData> Arguments
        {
            get
            {
                return _commandType
                    .GetProperties()
                    .Where(x => CustomAttributeExtensions.GetCustomAttribute<ArgumentAttribute>((MemberInfo) x) != null)
                    .Select(x => new ArgumentMetaData(x));
            }
        }

        #region equality members

        protected bool Equals(CommandMetaData other)
        {
            return Equals(_commandType, other._commandType);
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
            return Equals((CommandMetaData) obj);
        }

        public override int GetHashCode()
        {
            return (_commandType != null
                ? _commandType.GetHashCode()
                : 0);
        }

        public static bool operator ==(CommandMetaData left, CommandMetaData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CommandMetaData left, CommandMetaData right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}