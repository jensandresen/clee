using System;

namespace Clee
{
    public class ParseException : Exception
    {
        public ParseException(int errorOffset, string message) : base(message)
        {
            ErrorOffset = errorOffset;
        }

        public int ErrorOffset { get; private set; }
    }
}