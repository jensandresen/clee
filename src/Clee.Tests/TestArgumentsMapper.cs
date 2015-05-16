using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Clee.Tests
{
    public class TestArgumentsMapper
    {
        [Fact]
        public void returns_instance()
        {
            var sut = new ArgumentMapper();
            var result = sut.Map(typeof (RelaxedArgument), new Argument[0]);
            
            Assert.NotNull(result);
        }

        [Fact]
        public void returns_expected_type()
        {
            var sut = new ArgumentMapper();
            var result = sut.Map(typeof(RelaxedArgument), new Argument[0]);

            Assert.IsAssignableFrom<RelaxedArgument>(result);
        }

        [Fact]
        public void can_map_simple_string_property()
        {
            var sut = new ArgumentMapper();
            var result = (RelaxedArgument)sut.Map(typeof (RelaxedArgument), new[] {new Argument("text", "foo"),});

            Assert.Equal("foo", result.Text);
        }

        [Fact]
        public void returns_default_value_for_properties_marked_with_optional_attribute()
        {
            var sut = new ArgumentMapper();
            var result = (RelaxedArgument)sut.Map(typeof(RelaxedArgument), new Argument[0]);

            Assert.Null(result.Text);
        }

        [Fact]
        public void throws_exception_if_required_property_is_missing_a_value()
        {
            var sut = new ArgumentMapper();
            Assert.Throws<Exception>(() => sut.Map(typeof (StrictArgument), new Argument[0]));
        }


        private class RelaxedArgument : ICommandArguments
        {
            [Optional]
            public string Text { get; set; }
        }

        private class StrictArgument : ICommandArguments
        {
            public string Text { get; set; }
        }
    }

    public class ArgumentMapper
    {
        private static readonly Argument EmptyArgument = new Argument();

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
                    var optionalAttribute = property.GetCustomAttribute<OptionalAttribute>();

                    if (optionalAttribute == null)
                    {
                        throw new Exception(string.Format("Property {0} is NOT marked with the optional attribute and is therefore required.", name));
                    }
                }
                else
                {
                    property.SetValue(result, value.Value);
                }
            }

            return result;
        }
    }

    public class OptionalAttribute : Attribute
    {
         
    }
}