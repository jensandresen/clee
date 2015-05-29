using System;

namespace Clee.Parsing
{
    public class DefaultChangeTypeParser : IValueParser
    {
        private readonly Type _targetType;

        public DefaultChangeTypeParser(Type targetType)
        {
            _targetType = targetType;
        }

        public bool TryParse(string input, IFormatProvider format, out object result)
        {
            try
            {
                result = Convert.ChangeType(input, _targetType, format);
                return true;
            }
            catch
            {
            }

            result = null;
            return false;
        }
    }
}