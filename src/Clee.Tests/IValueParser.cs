using System;

namespace Clee.Tests
{
    public interface IValueParser
    {
        bool TryParse(string input, IFormatProvider format, out object result);
    }
}