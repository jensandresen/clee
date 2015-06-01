﻿using System;
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

        public CleeEngine(ICommandRegistry commandRegistry, ICommandFactory commandFactory, IArgumentMapper argumentMapper,
                      ICommandExecutor commandExecutor)
        {
            _registry = commandRegistry;
            _commandFactory = commandFactory;
            _mapper = argumentMapper;
            _commandExecutor = commandExecutor;
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
            var systemCommandInstance = CreateSystemCommandFrom(commandName);
            if (systemCommandInstance != null)
            {
                var argumentType2 = TypeUtils.ExtractArgumentTypesFromCommand(systemCommandInstance).First();
                var argumentInstance2 = _mapper.Map(argumentType2, args);
                _commandExecutor.Execute(systemCommandInstance, argumentInstance2);

                return;
            }

            var commandType = _registry.Find(commandName);
            if (commandType == null)
            {
                throw new NotSupportedException(string.Format("The command \"{0}\" is not currently supported.",
                    commandName));
            }

            var argumentType = TypeUtils.ExtractArgumentTypesFromCommand(commandType).First();
            var argumentInstance = _mapper.Map(argumentType, args);

            var commandInstance = _commandFactory.Resolve(commandType);
            if (commandInstance == null)
            {
                throw new Exception(string.Format("Command factory \"{1}\" was unable to resolve an instance for command type \"{0}\", it returned null instead.", commandType, _commandFactory.GetType().FullName));
            }

            try
            {
                _commandExecutor.Execute(commandInstance, argumentInstance);
            }
            finally
            {
                _commandFactory.Release(commandInstance);
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