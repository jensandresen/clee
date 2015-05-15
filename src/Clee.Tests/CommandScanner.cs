using System;
using System.Linq;
using System.Reflection;

namespace Clee.Tests
{
    public class CommandScanner
    {
        public Type[] Scan(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(x => x.IsClass)
                .Where(x => IsAssignableToGenericType(x, typeof (ICommand<>)))
                .ToArray();
        }

        public Type[] Scan(Assembly assembly, string namespaceQualifier)
        {
            return Scan(assembly)
                .Where(x => x.Namespace.StartsWith(namespaceQualifier))
                .ToArray();
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
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