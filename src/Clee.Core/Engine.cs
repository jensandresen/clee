using System;
using System.Linq;
using Clee.Parsing;
using Clee.Types;

namespace Clee
{
    public class Engine
    {
        private readonly ICommandRegistry _registry;
        private readonly ITypeFactory _typeFactory;
        private readonly IArgumentMapper _mapper;
        private readonly ICommandExecutor _commandExecutor;

        public Engine(ICommandRegistry commandRegistry, ITypeFactory typeFactory, IArgumentMapper argumentMapper, ICommandExecutor commandExecutor)
        {
            _registry = commandRegistry;
            _typeFactory = typeFactory;
            _mapper = argumentMapper;
            _commandExecutor = commandExecutor;
        }

        public void Execute(string input)
        {
            var commandName = CommandLineParser.ExtractCommandNameFrom(input);
            var commandType = _registry.Find(commandName);

            if (commandType == null)
            {
                throw new NotSupportedException(string.Format("The command \"{0}\" is not currently supported.", commandName));
            }

            var argumentType = TypeUtils.ExtractArgumentTypesFromCommand(commandType).First();
            var argumentValues = CommandLineParser.ExtractArgumentsFrom(input).ToArray();
            var argumentInstance = _mapper.Map(argumentType, argumentValues);

            var commandInstance = _typeFactory.Resolve(commandType);

            try
            {
                _commandExecutor.Execute(commandInstance, argumentInstance);
            }
            finally
            {
                _typeFactory.Release(commandInstance);
            }
        }

        public static Engine Create()
        {
            var builder = new EngineBuilder();
            return builder.Build();
        }

        public static Engine Create(Action<IEngineConfiguration> configure)
        {
            var builder = new EngineBuilder();
            configure(builder);
            return builder.Build();
        }
    }
}