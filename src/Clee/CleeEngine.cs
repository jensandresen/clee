using System;
using Clee.ErrorHandling;
using Clee.TypeResolving;

namespace Clee
{
    public class CleeEngine
    {
        private readonly IErrorHandlerEngine _errorHandlerEngine;
        private readonly ICommandResolver _commandResolver;

        public CleeEngine(ICommandResolver commandResolver)
        {
            _commandResolver = commandResolver;
            _errorHandlerEngine = new ErrorHandlerEngine(null);
        }

        private void InternalExecute(Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            command.Execute();
        }

        private bool TryExecute(Command command, out Exception error)
        {
            var success = false;

            try
            {
                InternalExecute(command);
                
                error = null;
                success = true;
            }
            catch (Exception err)
            {
                error = err;
            }

            return success;
        }

        public int Execute(Command command)
        {
            Exception exceptionThrown;

            if (TryExecute(command, out exceptionThrown))
            {
                return (int) CommandExecutionResultsType.Ok;
            }

            return _errorHandlerEngine.Handle(exceptionThrown);
        }
        
        public int Execute<T>() where T : Command
        {
            var command = _commandResolver.Resolve<T>();

            try
            {
                return Execute(command);
            }
            finally
            {
                _commandResolver.Release(command);
            }
        }
    }
}