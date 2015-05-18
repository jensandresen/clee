using System;

namespace Clee.Parsing
{
    public interface IValueParser
    {
        bool TryParse(string input, IFormatProvider format, out object result);
    }
}