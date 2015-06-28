using System;

namespace Clee
{
    public class CommandAttribute : Attribute
    {
        public string Description { get; set; }
    }
}