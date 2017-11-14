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

            var isNullable = targetType.IsGenericType &&
                             targetType.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (isNullable)
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }

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
}