using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clee.Mapping
{
    public class ArgumentMapper : IArgumentMapper
    {
        public void Map(Command command, IEnumerable<Argument> arguments)
        {
            var properties = GetCommandArgumentPropertiesFrom(command);
            var inputArguments = new HashSet<Argument>(arguments);

            var argumentDeclaredMultipleTimes = arguments
                .GroupBy(x => x.IsShortName ? x.Name : x.Name.ToLowerInvariant())
                .Where(x => x.Count() > 1)
                .FirstOrDefault();

            if (argumentDeclaredMultipleTimes != null)
            {
                throw new ArgumentDeclaredMultipleTimesException(argumentDeclaredMultipleTimes);
            }

            foreach (var property in properties)
            {
                var commandArgument = new ArgumentMetaData(property);

                var matchingInputArguments = inputArguments
                    .Where(x => IsMatch(x, commandArgument))
                    .ToArray();

                if (matchingInputArguments.Length > 1)
                {
                    throw new ArgumentDeclaredMultipleTimesException(matchingInputArguments);
                }

                var argument = matchingInputArguments.FirstOrDefault();

                if (argument == null && commandArgument.ArgumentIsRequired)
                {
                    throw new RequiredArgumentMissingException(commandArgument);
                }
                
                if (argument != null)
                {
                    inputArguments.Remove(argument);
                    AssignValueToCommandProperty(command, property, argument.Value);
                }
            }

            if (inputArguments.Count > 0)
            {
                var commandMetaData = new CommandMetaData(command.GetType());
                throw new UnknownCommandArgumentsException(commandMetaData, inputArguments);
            }
        }

        private void AssignValueToCommandProperty(Command command, PropertyInfo property, object value)
        {
            property.SetValue(command, value);
        }

        private static bool IsMatch(Argument argument, ArgumentMetaData metaData)
        {
            return metaData.ArgumentLongName
                           .Equals(argument.Name, StringComparison.InvariantCultureIgnoreCase) ||
                   metaData.ArgumentShortName.ToString()
                           .Equals(argument.Name);
        }

        private static IEnumerable<PropertyInfo> GetCommandArgumentPropertiesFrom(Command command)
        {
            var properties = command
                .GetType()
                .GetProperties()
                .Where(x => x.GetCustomAttribute<ArgumentAttribute>() != null)
                .ToArray();

            var argumentPropertyWithNonPublicSetter = properties
                .Where(x => x.GetSetMethod(nonPublic: false) == null)
                .FirstOrDefault();

            if (argumentPropertyWithNonPublicSetter != null)
            {
                var commandMetaData = new CommandMetaData(command.GetType());
                var argumentMetaData = new ArgumentMetaData(argumentPropertyWithNonPublicSetter);

                throw new UnavailableWriteAccessToCommandPropertyException(commandMetaData, argumentMetaData);
            }
            
            var ambigousCommandArgumentDefinition = GetAmbigousCommandArgumentDefinition(properties);

            if (ambigousCommandArgumentDefinition != null)
            {
                var commandMetaData = new CommandMetaData(command.GetType());
                throw new CommandHasAmbigousArgumentDefinitionException(commandMetaData, ambigousCommandArgumentDefinition);
            }

            return properties;
        }

        private static IEnumerable<ArgumentMetaData> GetAmbigousCommandArgumentDefinition(IEnumerable<PropertyInfo> properties)
        {
            var temp = properties as PropertyInfo[] ?? properties.ToArray();

            return GetAmbigousCommandArgumentDefinitionByLongName(temp) ??
                   GetAmbigousCommandArgumentDefinitionByShortName(temp);
        }

        private static IEnumerable<ArgumentMetaData> GetAmbigousCommandArgumentDefinitionByShortName(IEnumerable<PropertyInfo> properties)
        {
            var ambigousCommandArgumentDefinition = properties
                .Select(x => new ArgumentMetaData(x))
                .Where(x => x.ArgumentShortName.HasValue)
                .GroupBy(x => x.ArgumentShortName.Value)
                .Where(x => x.Count() > 1)
                .FirstOrDefault();

            return ambigousCommandArgumentDefinition;
        }

        private static IEnumerable<ArgumentMetaData> GetAmbigousCommandArgumentDefinitionByLongName(IEnumerable<PropertyInfo> properties)
        {
            var ambigousCommandArgumentDefinition = properties
                .Select(x => new ArgumentMetaData(x))
                .GroupBy(x => x.ArgumentLongName)
                .Where(x => x.Count() > 1)
                .FirstOrDefault();

            return ambigousCommandArgumentDefinition;
        }
    }
}