using System;
using System.Linq;
using System.Reflection;

namespace Clee
{
    public static class AttributeHelper
    {
        public static string GetDescription(Type implementationType, Type commandType)
        {
            var info = GetAttribute(implementationType, commandType);

            if (info != null)
            {
                return info.Description;
            }

            return implementationType.FullName;
        }

        public static string GetName(Type implementationType, Type commandType)
        {
            var info = GetAttribute(implementationType, commandType);

            if (info != null)
            {
                return info.Name;
            }

            return null;
        }

        private static CommandAttribute GetAttribute(Type implementationType, Type commandType)
        {
            if (!commandType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException(String.Format("The type {0} does not implement the expecte command interface {1}.", implementationType.FullName, commandType.FullName));
            }

            var info = GetMethodAttribute(implementationType, commandType);
            
            if (info != null)
            {
                return info;
            }

            return GetClassAttribute(implementationType);
        }

        private static CommandAttribute GetClassAttribute(Type implementationType)
        {
            return implementationType.GetCustomAttribute<CommandAttribute>();
        }

        private static CommandAttribute GetMethodAttribute(Type implementationType, Type commandType)
        {
            var method = implementationType
                .GetInterfaceMap(commandType)
                .TargetMethods
                .Single();

            return method.GetCustomAttribute<CommandAttribute>();
        }
    }
}