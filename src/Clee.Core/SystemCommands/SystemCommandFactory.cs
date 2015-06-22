using System;
using System.Collections.Generic;
using System.Linq;

namespace Clee.SystemCommands
{
    internal class SystemCommandFactory : ICommandFactory
    {
        private readonly Dictionary<Type, object> _knownDependencies = new Dictionary<Type, object>();

        public void RegisterInstance<T>(T instance)
        {
            RegisterInstance(typeof(T), instance);
        }

        public void RegisterInstance(Type serviceType, object instance)
        {
            _knownDependencies.Add(serviceType, instance);
        }

        public object Resolve(Type commandType)
        {
            if (commandType == null)
            {
                return null;
            }

            var constructor = commandType
                .GetConstructors()
                .OrderBy(x => x.GetParameters().Length)
                .First();

            var parameters = new LinkedList<object>();

            foreach (var p in constructor.GetParameters())
            {
                object result;

                if (_knownDependencies.TryGetValue(p.ParameterType, out result))
                {
                    parameters.AddLast(result);
                }
                else
                {
                    throw new Exception(string.Format("Unable to resolve dependency {0} of system command {1}.", p.ParameterType.FullName, commandType.FullName));
                }
            }

            return constructor.Invoke(parameters.ToArray());
        }

        public void Release(object obj)
        {
            throw new NotImplementedException();
        }
    }
}