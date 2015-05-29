using System;
using System.Linq;

namespace Clee.Parsing
{
    public class TryParseConventionParser : IValueParser
    {
        private readonly Type _targetType;

        public TryParseConventionParser(Type targetType)
        {
            _targetType = targetType;
        }

        public bool TryParse(string input, IFormatProvider format, out object result)
        {
            var tryParseMethod = _targetType
                .GetMethods()
                .Where(x => x.IsStatic)
                .Where(x => x.IsPublic)
                .Where(x => x.Name.Equals("TryParse"))
                .FirstOrDefault();

            if (tryParseMethod != null)
            {
                var parameters = new object[] { input, null };
                var success = (bool)tryParseMethod.Invoke(null, parameters);

                if (success)
                {
                    result = parameters[1];
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}