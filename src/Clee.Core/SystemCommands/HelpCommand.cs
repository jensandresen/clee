using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Clee.SystemCommands
{
    internal class HelpCommand : ICommand<EmptyArgument>
    {
        private readonly ICommandRegistry _registry;

        public HelpCommand(ICommandRegistry registry)
        {
            _registry = registry;
        }

        public void Execute(EmptyArgument args)
        {
            PrintUsageInformation();
            Console.WriteLine();

            var systemCommands = GetSystemCommands();
            var customCommands = GetCustomCommands();
            PadCommandNames(systemCommands, customCommands);

            PrintCommands("System commands:", systemCommands);
            Console.WriteLine();

            PrintCommands("Available commands:", customCommands);
            Console.WriteLine("");
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

        private void PrintVersion()
        {
            var version = Assembly
                .GetExecutingAssembly()
                .GetName()
                .Version;

            Console.WriteLine("Provided by Clee v{0}.{1}.{2}",
                version.Major,
                version.Minor,
                version.Revision
                );
        }

        private static void PrintUsageInformation()
        {
            var applicationName = Environment
                .GetCommandLineArgs()
                .First();

            applicationName = Path.GetFileNameWithoutExtension(applicationName);

            Console.WriteLine("Usage: {0} <command> [<args>]", applicationName);
        }

        private void PrintCommands(string headline, IEnumerable<CommandInformation> commands)
        {
            Console.WriteLine(headline);

            foreach (var r in commands)
            {
                Console.WriteLine("  {0}\t{1}", r.Name, r.Description);
            }
        }
    }
}