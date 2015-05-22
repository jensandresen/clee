using System;

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

            foreach (var r in _registry.GetAll())
            {
                Console.WriteLine("\t{0}\t{1}", r.CommandName, r.ImplementationType.FullName);
            }
        }
    }
}