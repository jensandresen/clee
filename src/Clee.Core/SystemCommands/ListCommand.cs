using System;
using System.Linq;

namespace Clee.SystemCommands
{
    [Obsolete("Will be removed in version 2.0.0")]
    [Command(Description = "Displays a list of all the commands that are available")]
    public class ListCommand : ICommand<EmptyArgument>
    {
        private readonly ICommandRegistry _registry;
        private readonly SystemCommandRegistry _systemRegistry;

        public ListCommand(ICommandRegistry registry, SystemCommandRegistry systemRegistry)
        {
            _registry = registry;
            _systemRegistry = systemRegistry;
        }

        public void Execute(EmptyArgument args)
        {
            Console.WriteLine("Available commands:");

            var systemCommands = GetSystemCommands();
            var customCommands = GetCustomCommands();
            PadCommandNames(systemCommands, customCommands);

            foreach (var command in customCommands)
            {
                Console.WriteLine("  {0}\t{1}", command.Name, command.Description);
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
    }
}