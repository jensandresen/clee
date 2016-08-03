using System;

namespace Clee
{
    public class CommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}