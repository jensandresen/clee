using System;
using System.Collections.Generic;

namespace Clee.Mapping
{
    public class UnknownCommandArgumentsException : Exception
    {
        public UnknownCommandArgumentsException(CommandMetaData command, IEnumerable<Argument> argument)
        {
            Command = command;
            Argument = argument;
        }

        public CommandMetaData Command { get; private set; }
        public IEnumerable<Argument> Argument { get; private set; }
    }
}