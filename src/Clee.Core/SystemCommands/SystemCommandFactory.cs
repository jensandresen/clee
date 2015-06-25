using System;
using System.Collections.Generic;
using System.Linq;

namespace Clee.SystemCommands
{
    public class SystemCommandFactory : ICommandFactory
    {
        private readonly Dictionary<Type, object> _knownDependencies = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Func<object>> _factoryMethods = new Dictionary<Type, Func<object>>(); 
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
                var result = GetInstance(p.ParameterType);

                if (result != null)
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

        private object GetInstance(Type type)
        {
            object result1;
            if (_knownDependencies.TryGetValue(type, out result1))
            {
                return result1;
            }

            Func<object> result2;
            if (_factoryMethods.TryGetValue(type, out result2))
            {
                return result2();
            }

            return null;
        }

        public void Release(object obj)
        {
            // currently all system commands have singleton dependencies
        }

        public void RegisterFactoryMethod<T>(Func<T> creator)
        {
            _factoryMethods.Add(typeof(T), creator as Func<object>);
        }
    }
}