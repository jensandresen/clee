using System;

namespace Clee.Mapping
{
    public class RequiredArgumentMissingException : Exception
    {
        public RequiredArgumentMissingException(ArgumentMetaData argument)
        {
            Argument = argument;
        }

        public ArgumentMetaData Argument { get; private set; }
    }
}