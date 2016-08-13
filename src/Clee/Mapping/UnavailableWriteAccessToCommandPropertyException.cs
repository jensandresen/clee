using System;

namespace Clee.Mapping
{
    public class UnavailableWriteAccessToCommandPropertyException : Exception
    {
        public UnavailableWriteAccessToCommandPropertyException(CommandMetaData command, ArgumentMetaData argument)
        {
            Command = command;
            Argument = argument;
        }

        public CommandMetaData Command { get; private set; }
        public ArgumentMetaData Argument { get; private set; }
    }
}