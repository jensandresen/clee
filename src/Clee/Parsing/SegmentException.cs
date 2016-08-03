using System;

namespace Clee.Parsing
{
    public class SegmentException : Exception
    {
        public SegmentException(int errorOffset, string input) : base()
        {
            ErrorOffset = errorOffset;
            Input = input;
        }

        public int ErrorOffset { get; private set; }
        public string Input { get; private set; }
    }
}