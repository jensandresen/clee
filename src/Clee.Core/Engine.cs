using System;
using System.Linq;
using System.Reflection;
using Clee.Parsing;
using Clee.Types;

namespace Clee
{
    internal class EmptyArgument : ICommandArguments { }
    internal class ListCommand : ICommand<EmptyArgument>
    {
        private readonly ICommandRegistry _registry;

        public ListCommand(ICommandRegistry registry)
        {
            _registry = registry;
        }

        public void Execute(EmptyArgument args)
        {
            Console.WriteLine("Available commands:");

            foreach (var r in _registry.GetAll())
            {
                Console.WriteLine("\t{0}\t{1}", r.CommandName, r.ImplementationType.FullName);
            }
        }
    }

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
            var argumentValues = CommandLineParser.ExtractArgumentsFrom(input).ToArray();

            var systemCommandInstance = CreateSystemCommandFrom(commandName);
            if (systemCommandInstance != null)
            {
                var argumentType2 = TypeUtils.ExtractArgumentTypesFromCommand(systemCommandInstance).First();
                var argumentInstance2 = _mapper.Map(argumentType2, argumentValues);
                _commandExecutor.Execute(systemCommandInstance, argumentInstance2);

                return;
            }

            var commandType = _registry.Find(commandName);
            if (commandType == null)
            {
                throw new NotSupportedException(string.Format("The command \"{0}\" is not currently supported.", commandName));
            }

            var argumentType = TypeUtils.ExtractArgumentTypesFromCommand(commandType).First();
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

        private object CreateSystemCommandFrom(string commandName)
        {
            switch (commandName)
            {
                case "--list":
                    return new ListCommand(_registry);
            }

            return null;
        }

        public static Engine CreateDefault()
        {
            return Create(cfg =>
            {
                var registry = new DefaultCommandRegistry();
                var assembly = Assembly.GetEntryAssembly();
                var commandTypes = new CommandScanner().Scan(assembly);
                registry.Register(commandTypes);

                cfg.WithRegistry(registry);
            });
        }

        public static Engine Create(Action<IEngineConfiguration> configure)
        {
            var builder = new EngineBuilder();
            configure(builder);

            return builder.Build();
        }
    }
}