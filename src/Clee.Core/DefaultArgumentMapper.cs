using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Clee.Parsing;

namespace Clee
{
    public class DefaultArgumentMapper : IArgumentMapper
    {
        private static readonly Argument EmptyArgument = new Argument();
        private readonly Dictionary<Type, IValueParser> _valueParsers = new Dictionary<Type, IValueParser>();

        public DefaultArgumentMapper()
        {
            AddParser<Guid>(new GuidParser());
            AddParser<bool>(new BooleanParser());
        }

        public void AddParser<T>(IValueParser parser)
        {
            AddParser(typeof(T), parser);
        }

        public void AddParser(Type targetType, IValueParser parser)
        {
            _valueParsers.Add(targetType, parser);
        }

        public object Map(Type argumentType, Argument[] argumentValues)
        {
            var result = Activator.CreateInstance(argumentType);

            var properties = argumentType
                .GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.CanWrite);

            foreach (var property in properties)
            {
                var name = property.Name;
                var value = argumentValues.SingleOrDefault(x => name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));
                var isValueEmpty = value.Equals(EmptyArgument);

                if (isValueEmpty)
                {
                    if (property.PropertyType == typeof (bool))
                    {
                        property.SetValue(result, false);
                    }
                    else
                    {
                        if (!IsOptional(property))
                        {
                            throw new Exception(string.Format("Property {0} is NOT marked as optional and is therefore required.", name));
                        }
                    }
                }
                else
                {
                    var newValue = ConvertInputValueToTargetType(value.Value, property.PropertyType, CultureInfo.InvariantCulture);
                    property.SetValue(result, newValue);
                }
            }

            return result;
        }

        private static bool IsOptional(PropertyInfo property)
        {
            var valueAttribute = property.GetCustomAttribute<ValueAttribute>();

            if (valueAttribute != null)
            {
                return valueAttribute.IsOptional;
            }

            return false;
        }

        private object ConvertInputValueToTargetType(string inputValue, Type targetType, IFormatProvider format)
        {
            object result;
            IValueParser valueParser;

            if (_valueParsers.TryGetValue(targetType, out valueParser))
            {
                if (valueParser.TryParse(inputValue, format, out result))
                {
                    return result;
                }
            }

            valueParser = new DefaultChangeTypeParser(targetType);
            if (valueParser.TryParse(inputValue, format, out result))
            {
                return result;
            }

            valueParser = new TryParseConventionParser(targetType);
            if (valueParser.TryParse(inputValue, format, out result))
            {
                return result;
            }

            valueParser = new SimpleConstructorParser(targetType);
            if (valueParser.TryParse(inputValue, format, out result))
            {
                return result;
            }

            throw new NotSupportedException(string.Format("The value \"{0}\" could not be converted to the target type of {1}. Consider adding a specific value parser or a TryParse method on the target type.", inputValue, targetType.FullName));
        }
    }

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