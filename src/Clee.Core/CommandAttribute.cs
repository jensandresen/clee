using System;

namespace Clee
{
    public class CommandAttribute : Attribute
    {
        public string Description { get; set; }
        public string Name { get; set; }
    }
}