using System;
using System.Collections.Generic;

namespace Clee.Mapping
{
    public class ArgumentDeclaredMultipleTimesException : Exception
    {
        public ArgumentDeclaredMultipleTimesException(IEnumerable<Argument> arguments)
        {
            Arguments = arguments;
        }
        
        public IEnumerable<Argument> Arguments { get; private set; }
    }
}