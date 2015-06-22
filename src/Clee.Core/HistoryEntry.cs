using System;

namespace Clee
{
    public class HistoryEntry
    {
        public HistoryEntry(string commandName, Type implementationType)
        {
            CommandName = commandName;
            ImplementationType = implementationType;
        }

        public string CommandName { get; private set; }
        public Type ImplementationType { get; private set; }
    }
}