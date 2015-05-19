using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Clee.Types;

namespace Clee
{
    public class DefaultCommandExecutor
    {
        private readonly Dictionary<Type, MethodInfo> _executeSignatures = new Dictionary<Type, MethodInfo>();

        public void Execute(object command, object arguments)
        {
            var commandType = command.GetType();

            var isRealCommand = CommandScanner.IsAssignableToGenericType(commandType, typeof(ICommand<>));
            if (!isRealCommand)
            {
                throw new NotSupportedException(string.Format("Command {0} does not implement required interface {1}", commandType.FullName, typeof(ICommand<>).FullName));
            }

            var isRealArgument = arguments is ICommandArguments;
            if (!isRealArgument)
            {
                throw new NotSupportedException(string.Format("Arguments does not implement required interface {0}", typeof(ICommandArguments).FullName));
            }

            var method = GetExecuteMethodFor(arguments);
            method.Invoke(command, new[] { arguments });
        }

        private MethodInfo GetExecuteMethodFor(object arguments)
        {
            var argumentsType = arguments.GetType();

            MethodInfo method = null;

            if (!_executeSignatures.TryGetValue(argumentsType, out method))
            {
                method = typeof(ICommand<>)
                    .MakeGenericType(argumentsType)
                    .GetMethods()
                    .Where(m => m.IsPublic)
                    .Where(m => m.Name == "Execute")
                    .Single();

                _executeSignatures.Add(argumentsType, method);
            }

            return method;
        }
    }
}