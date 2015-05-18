using System;

namespace Clee
{
    public class ValueAttribute : Attribute
    {
        public bool IsOptional { get; set; }
        public string Format { get; set; }
        public string DefaultValue { get; set; }
    }
}