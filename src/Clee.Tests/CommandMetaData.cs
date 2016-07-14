using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clee.Tests
{
    public class CommandMetaData
    {
        private readonly Type _commandType;

        public CommandMetaData(Type commandType)
        {
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
    }
}