﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Clee.Configurations;
using Clee.Parsing;
using Clee.SystemCommands;

namespace Clee
{
    public class CleeEngine
    {
        private readonly ICommandRegistry _registry;
        private readonly ICommandFactory _commandFactory;
        private readonly IArgumentMapper _mapper;
        private readonly ICommandExecutor _commandExecutor;
        private readonly SystemCommandRegistry _systemRegistry;
        private readonly LinkedList<HistoryEntry> _history;
        private readonly SystemCommandFactory _systemCommandFactory;
        private IOutputWriter _outputWriter;

        public CleeEngine(ICommandRegistry commandRegistry, ICommandFactory commandFactory, 
            IArgumentMapper argumentMapper, ICommandExecutor commandExecutor)
        {
            _registry = commandRegistry;
            _commandFactory = commandFactory;
            _mapper = argumentMapper;
            _commandExecutor = commandExecutor;
            
            _systemRegistry = SystemCommandRegistry.CreateAndInitialize();

            _systemCommandFactory = new SystemCommandFactory();
            _systemCommandFactory.RegisterInstance<ICommandRegistry>(_registry);
            _systemCommandFactory.RegisterInstance<ICommandFactory>(_commandFactory);
            _systemCommandFactory.RegisterInstance<IArgumentMapper>(_mapper);
            _systemCommandFactory.RegisterInstance<ICommandExecutor>(_commandExecutor);
            _systemCommandFactory.RegisterInstance<SystemCommandRegistry>(_systemRegistry);
            _systemCommandFactory.RegisterFactoryMethod<IOutputWriter>(() => _outputWriter);

            _history = new LinkedList<HistoryEntry>();
            _outputWriter = new DefaultOutputWriter();
        }

        public IArgumentMapper Mapper
        {
            get { return _mapper; }
        }

        public ICommandRegistry Registry
        {
            get { return _registry; }
        }

        public ICommandFactory Factory
        {
            get { return _commandFactory; }
        }

        public IEnumerable<HistoryEntry> History
        {
            get { return _history; }
        }

        public void Execute(string input)
        {
            var commandName = CommandLineParser.ExtractCommandNameFrom(input);
            var argumentValues = CommandLineParser.ExtractArgumentsFrom(input).ToArray();

            Execute(commandName, argumentValues);
        }

        public void Execute(string[] input)
        {
            var commandName = input[0];
            var temp = input
                .Skip(1)
                .Select(x =>
                {
                    if (x.StartsWith("-"))
                    {
                        return x;
                    }

                    return string.Format("\"{0}\"", x);
                });

            var args = string.Join(" ", temp);

            var argumentValues = CommandLineParser.ParseArguments(args).ToArray();

            Execute(commandName, argumentValues);
        }

        public void Execute(string commandName, Argument[] args)
        {
            CreateCommandFrom(commandName, commandInstance =>
            {
                var argumentType = TypeUtils.ExtractArgumentTypesFromCommand(commandInstance).First();
                var argumentInstance = _mapper.Map(argumentType, args);

                _commandExecutor.Execute(commandInstance, argumentInstance);

                _history.AddLast(new HistoryEntry(
                    commandName: commandName,
                    implementationType: commandInstance.GetType()
                    ));
            });
        }

        private void CreateCommandFrom(string commandName, Action<object> commandHandler)
        {
            var systemCommandType = _systemRegistry.Find(commandName);
            var systemCommandInstance = _systemCommandFactory.Resolve(systemCommandType);
            if (systemCommandInstance != null)
            {
                try
                {
                    commandHandler(systemCommandInstance);
                }
                finally
                {
                    _systemCommandFactory.Release(systemCommandInstance);
                }

                return;
            }

            var commandType = _registry.Find(commandName);
            if (commandType == null)
            {
                throw new NotSupportedException(string.Format("The command \"{0}\" is not currently supported.", commandName));
            }

            var commandInstance = _commandFactory.Resolve(commandType);
            if (commandInstance == null)
            {
                throw new Exception(string.Format("Command factory \"{1}\" was unable to resolve an instance for command type \"{0}\", it returned null instead.", commandType, _commandFactory.GetType().FullName));
            }

            try
            {
                commandHandler(commandInstance);
            }
            finally
            {
                _commandFactory.Release(commandInstance);
            }
        }

        public void SetOutputWriter(IOutputWriter outputWriter)
        {
            _outputWriter = outputWriter;
        }

        public static CleeEngine CreateDefault()
        {
            var assembly = Assembly.GetCallingAssembly();

            return Create(cfg =>
            {
                cfg.Registry(r => r.RegisterFromAssembly(assembly));
            });
        }

        public static CleeEngine Create(Action<IEngineConfiguration> configure)
        {
            var builder = new EngineBuilder();
            configure(builder);

            return builder.Build();
        }
    }
}