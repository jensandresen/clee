using System;
using System.Linq;
using System.Reflection;

namespace Clee
{
    public static class AttributeHelper
    {
        public static string GetDescription(Type implementationType, Type commandType)
        {
            if (!commandType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException(String.Format("The type {0} does not implement the expecte command interface {1}.", implementationType.FullName, commandType.FullName));
            }

            var info = GetMethodInformation(implementationType, commandType);
            if (info != null)
            {
                return info.Description;
            }

            info = implementationType.GetCustomAttribute<CommandAttribute>();
            if (info != null)
            {
                return info.Description;
            }

            return implementationType.FullName;
        }

        private static CommandAttribute GetMethodInformation(Type implementationType, Type commandType)
        {
            var method = implementationType
                .GetInterfaceMap(commandType)
                .TargetMethods
                .Single();

            return method.GetCustomAttribute<CommandAttribute>();
        }
    }
}