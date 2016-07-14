using System;

namespace Clee.Tests
{
    public class CommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}