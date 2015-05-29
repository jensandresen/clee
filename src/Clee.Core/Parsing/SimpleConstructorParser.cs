using System;
using System.Linq;

namespace Clee.Parsing
{
    public class SimpleConstructorParser : IValueParser
    {
        private readonly Type _targetType;

        public SimpleConstructorParser(Type targetType)
        {
            _targetType = targetType;
        }

        public bool TryParse(string input, IFormatProvider format, out object result)
        {
            var constructors = _targetType
                .GetConstructors()
                .Where(x => x.IsPublic)
                .Where(x => x.GetParameters().Length == 1)
                .ToArray();

            if (constructors.Length == 1)
            {
                var constructor = constructors[0];

                var parameter = constructor.GetParameters().Single();
                var parser = new DefaultChangeTypeParser(parameter.ParameterType);

                object value;

                if (parser.TryParse(input, format, out value))
                {
                    result = constructor.Invoke(new[] {value});
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}