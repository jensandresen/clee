using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Clee.SystemCommands
{
    public class HelpCommand : ICommand<EmptyArgument>
    {
        private readonly ICommandRegistry _registry;
        private readonly SystemCommandRegistry _systemRegistry;
        private readonly IOutputWriter _outputWriter;

        public HelpCommand(ICommandRegistry registry, SystemCommandRegistry systemRegistry, IOutputWriter outputWriter)
        {
            _registry = registry;
            _systemRegistry = systemRegistry;
            _outputWriter = outputWriter;
        }

        public void Execute(EmptyArgument args)
        {
            PrintUsageInformation();
            _outputWriter.WriteLine();

            var systemCommands = GetSystemCommands();
            var customCommands = GetCustomCommands();
            PadCommandNames(systemCommands, customCommands);

            PrintCommands("System commands:", systemCommands);
            _outputWriter.WriteLine();

            PrintCommands("Available commands:", customCommands);
            _outputWriter.WriteLine("");
        }

        private static void PadCommandNames(CommandInformation[] systemCommands, CommandInformation[] customCommands)
        {
            var allCommands = Enumerable
                .Concat(systemCommands, customCommands)
                .ToArray();

            var longestName = allCommands.Max(x => x.Name.Length);

            if (longestName < 10)
            {
                longestName = 10;
            }

            foreach (var command in allCommands)
            {
                if (command.Name.Length < longestName)
                {
                    command.Name = command.Name.PadRight(longestName);
                }
            }
        }

        private CommandInformation[] GetCustomCommands()
        {
            return ExtractCommandInformationFrom(_registry);
        }

        private CommandInformation[] GetSystemCommands()
        {
            return ExtractCommandInformationFrom(_systemRegistry);
        }

        private static CommandInformation[] ExtractCommandInformationFrom(ICommandRegistry registry)
        {
            var commands = registry
                .GetAll()
                .Select(x => new CommandInformation
                {
                    Name = x.CommandName,
                    Description = x.ImplementationType.FullName
                })
                .ToArray();

            return commands;
        }

        private void PrintVersion()
        {
            var version = Assembly
                .GetExecutingAssembly()
                .GetName()
                .Version;

            _outputWriter.WriteLine("Provided by Clee v{0}.{1}.{2}",
                version.Major,
                version.Minor,
                version.Revision
                );
        }

        private void PrintUsageInformation()
        {
            var applicationName = Environment
                .GetCommandLineArgs()
                .First();

            applicationName = Path.GetFileNameWithoutExtension(applicationName);

            _outputWriter.WriteLine("Usage: {0} <command> [<args>]", applicationName);
        }

        private void PrintCommands(string headline, IEnumerable<CommandInformation> commands)
        {
            _outputWriter.WriteLine(headline);

            foreach (var r in commands)
            {
                _outputWriter.WriteLine("  {0}\t{1}", r.Name, r.Description);
            }
        }
    }
}