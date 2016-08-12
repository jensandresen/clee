using System;
using Clee.ErrorHandling;
using Clee.Mapping;
using Clee.Parsing;
using Clee.Routing;
using Clee.TypeResolving;

namespace Clee
{
    public class CleeEngine
    {
        private readonly IErrorHandlerEngine _errorHandlerEngine;
        private readonly ICommandResolver _commandResolver;
        private readonly IRouteFinder _routeFinder;
        private readonly IParser _parser;
        private readonly IArgumentMapper _argumentMapper;

        public CleeEngine(ICommandResolver commandResolver, IRouteFinder routeFinder, IParser parser, IArgumentMapper argumentMapper)
        {
            _commandResolver = commandResolver;
            _routeFinder = routeFinder;
            _parser = parser;
            _argumentMapper = argumentMapper;
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

        public int Execute(string input)
        {
            var result = _parser.Parse(input);
            var route = _routeFinder.FindRoute(result.Path);

            var command = _commandResolver.Resolve(route.CommandType);

            try
            {
                _argumentMapper.Map(command, result.Arguments);
                return Execute(command);
            }
            catch (Exception err)
            {
                return _errorHandlerEngine.Handle(err);
            }
            finally
            {
                _commandResolver.Release(command);
            }
        }
    }
}