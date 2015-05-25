using System.Text.RegularExpressions;
using Clee.Types;
using Moq;
using Xunit;

namespace Clee.Tests
{
    public class TestEngine
    {
        [Fact]
        public void testname()
        {
            var registry = new DefaultCommandRegistry();
            registry.Register(typeof(FooCommand));

            var engine = Engine.Create(cfg =>
            {
                cfg.WithRegistry(registry);
            });

            engine.Execute("foo");
        }

        private class FooData : ICommandArguments
        {
            [Value(IsOptional = true)]
            public string Name { get; set; }
        }

        private class FooCommand : ICommand<FooData>
        {
            public void Execute(FooData args)
            {
                
            }
        }

        private class CustomValueType
        {
            private readonly int _value;

            public CustomValueType(int value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return string.Format("X{0:D5}", _value);
            }
        }
    }

    public class TestEngine_CustomValueTypes
    {
        [Fact(Skip = "later")]
        public void testname()
        {
            var mock = new Mock<ICommand<FooData>>();

            var registry = new DefaultCommandRegistry();
            registry.Register("foo", mock.Object.GetType());

            var factory = new ManualCommandFactory();
            factory.Add<ICommand<FooData>>(() => mock.Object);

            var engine = Engine.Create(cfg =>
            {
                cfg.WithRegistry(registry);
                cfg.WithFactory(factory);
            });

            engine.Execute("foo -id 1");
        }

        private class FooData : ICommandArguments
        {
            public string Id { get; set; }
        }

        private class FooCommand : ICommand<FooData>
        {
            public void Execute(FooData args)
            {
                
            }
        }

        private class CustomValueType
        {
            private readonly int _value;

            public CustomValueType(int value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return string.Format("X{0:D5}", _value);
            }

            public static bool TryParse(string input, out CustomValueType instance)
            {
                var isValid = Regex.IsMatch(input, @"X?\d+");

                if (isValid)
                {
                    var number = Regex.Replace(input, @"X?(\d+)", "$1");
                    int value;

                    if (int.TryParse(number, out value))
                    {
                        instance = new CustomValueType(value);
                        return true;
                    }
                }

                instance = null;
                return false;
            }
        }
    }
}