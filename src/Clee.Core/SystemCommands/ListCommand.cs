using System;
using System.Linq;

namespace Clee.SystemCommands
{
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
            var customCommands = _registry
                .GetAll()
                .Select(x => new CommandInformation
                {
                    Name = x.CommandName,
                    Description = x.ImplementationType.FullName
                })
                .ToArray();

            return customCommands;
        }

        private static CommandInformation[] GetSystemCommands()
        {
            var systemCommands = new[]
            {
                new CommandInformation
                {
                    Name = "--help",
                    Description = "Displays this help information"
                },
                new CommandInformation
                {
                    Name = "--list",
                    Description = "Shows a list of all the registered commands"
                },
            };
            return systemCommands;
        }

        private static void PadCommandNames(CommandInformation[] systemCommands, CommandInformation[] customCommands)
        {
            var allCommands = Enumerable
                .Concat(systemCommands, customCommands)
                .ToArray();

            var longestName = allCommands.Max(x => x.Name.Length);

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