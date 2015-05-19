using System;
using System.Linq;

namespace Clee.Types
{
    public static class TypeUtils
    {
        public static Type[] ExtractArgumentTypesFromCommand(object command)
        {
            var temp = command
                .GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(ICommand<>))
                .ToArray();

            return temp
                .SelectMany(x => x.GenericTypeArguments)
                .Where(x => typeof(ICommandArguments).IsAssignableFrom(x))
                .ToArray();
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            Type baseType = givenType.BaseType;

            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}