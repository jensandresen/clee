using System;

namespace Clee.Tests
{
    public class BooleanParser : IValueParser
    {
        public bool TryParse(string input, IFormatProvider format, out object result)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                result = true;
                return true;
            }

            bool value;
            var isSuccess = bool.TryParse(input, out value);

            result = value;
            return isSuccess;
        }
    }
}