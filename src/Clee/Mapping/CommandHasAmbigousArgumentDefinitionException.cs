using System;
using System.Collections.Generic;

namespace Clee.Mapping
{
    public class CommandHasAmbigousArgumentDefinitionException : Exception
    {
        public CommandHasAmbigousArgumentDefinitionException(CommandMetaData command, IEnumerable<ArgumentMetaData> ambigousArgumentDefinition)
        {
            Command = command;
            AmbigousArgumentDefinition = ambigousArgumentDefinition;
        }

        public CommandMetaData Command { get; private set; }
        public IEnumerable<ArgumentMetaData> AmbigousArgumentDefinition { get; private set; }
    }
}