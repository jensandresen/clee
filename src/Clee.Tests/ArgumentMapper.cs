using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Clee.Tests
{
    public class ArgumentMapper
    {
        private static readonly Argument EmptyArgument = new Argument();
        private readonly Dictionary<Type, IValueParser> _valueParsers = new Dictionary<Type, IValueParser>();

        public ArgumentMapper()
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
                        var optionalAttribute = property.GetCustomAttribute<OptionalAttribute>();

                        if (optionalAttribute == null)
                        {
                            throw new Exception(string.Format("Property {0} is NOT marked with the optional attribute and is therefore required.", name));
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

        private object ConvertInputValueToTargetType(string inputValue, Type targetType, IFormatProvider format)
        {
            IValueParser valueParser;
            
            if (_valueParsers.TryGetValue(targetType, out valueParser))
            {
                object value;

                if (valueParser.TryParse(inputValue, format, out value))
                {
                    return value;
                }
            }

            return Convert.ChangeType(inputValue, targetType, format);
        }
    }
}