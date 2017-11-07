using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clee
{
    public class DefaultCommandExecutor : ICommandExecutor
    {
        private readonly Dictionary<Type, MethodInfo> _executeSignatures = new Dictionary<Type, MethodInfo>();

        public void Execute(object command, object arguments)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command", "The command object is null and cannot be executed by the executor.");
            }
            
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments", "The arguments object is null.");
            }

            var commandType = command.GetType();

            var isRealCommand = TypeUtils.IsAssignableToGenericType(commandType, typeof(ICommand<>));
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

            try
            {
                method.Invoke(command, new[] { arguments });
            }
            catch (TargetInvocationException err)
            {
                if (err.InnerException != null)
                {
                    throw err.InnerException;
                }
                else
                {
                    throw;
                }
            }
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