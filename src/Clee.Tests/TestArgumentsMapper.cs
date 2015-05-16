using System;
using System.Linq;
using Xunit;

namespace Clee.Tests
{
    public class TestArgumentsMapper
    {
        [Fact]
        public void returns_instance()
        {
            var sut = new ArgumentMapper();
            var result = sut.Map(typeof (FooArgument), new Argument[0]);
            
            Assert.NotNull(result);
        }

        [Fact]
        public void returns_expected_type()
        {
            var sut = new ArgumentMapper();
            var result = sut.Map(typeof(FooArgument), new Argument[0]);

            Assert.IsAssignableFrom<FooArgument>(result);
        }

        [Fact]
        public void can_map_simple_string_property()
        {
            var sut = new ArgumentMapper();
            var result = (FooArgument)sut.Map(typeof (FooArgument), new[] {new Argument("text", "foo"),});

            Assert.Equal("foo", result.Text);
        }

        private class FooArgument : ICommandArguments
        {
            public string Text { get; set; }
        }
    }

    public class ArgumentMapper
    {
        public object Map(Type argumentType, Argument[] argumentValues)
        {
            var result = Activator.CreateInstance(argumentType);

            if (argumentValues.Length == 0)
            {
                return result;
            }

            var properties = argumentType
                .GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.CanWrite);

            foreach (var property in properties)
            {
                var name = property.Name;
                var value = argumentValues.Single(x => name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));

                var empty = new Argument();

                if (!value.Equals(empty))
                {
                    property.SetValue(result, value.Value);
                }
            }

            return result;
        }
    }
}