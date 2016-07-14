using System;

namespace Clee.Tests
{
    public class ArgumentAttribute : Attribute
    {
        public static readonly char EmptyChar = (char) 0;

        public ArgumentAttribute()
        {
            ShortName = EmptyChar;
            IsRequired = true;
        }

        public string LongName { get; set; }
        public char ShortName { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
    }
}