using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Clee.Tests.TestDoubles;
using Clee.Types;
using Xunit;

namespace Clee.Tests
{
    public class TestCommandRegistry
    {
        [Fact]
        public void Find_returns_expected_when_no_commands_are_registered()
        {
            var sut = new CommandRegistry();
            var result = sut.Find("foo");

            Assert.Null(result);
        }

        [Fact]
        public void GetAll_returns_expected_when_empty()
        {
            var sut = new CommandRegistry();
            var result = sut.GetAll();

            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_returns_expected_when_single_command_is_added()
        {
            var sut = new CommandRegistry();
            sut.Register(typeof (FooCommand));
            var result = sut.GetAll();

            Assert.Equal(new[]{typeof(FooCommand)}, result);
        }

        [Fact]
        public void GetAll_returns_expected_when_multiple_commands_are_added()
        {
            var sut = new CommandRegistry();
            sut.Register(typeof (FooCommand));
            sut.Register(typeof (BarCommand));
            var result = sut.GetAll();

            Assert.Equal(new[]
            {
                typeof(FooCommand),
                typeof(BarCommand),
            }, result);
        }

        [Fact]
        public void can_add_multiple_commands_at_once()
        {
            var sut = new CommandRegistry();
            sut.Register(new[]
            {
                typeof(FooCommand),
                typeof(BarCommand),
            });
            
            var result = sut.GetAll();

            Assert.Equal(new[]
            {
                typeof(FooCommand),
                typeof(BarCommand),
            }, result);
        }

        [Fact]
        public void throws_exception_if_command_is_not_a_real_command()
        {
            var invalidCommandType = typeof (string);
            var sut = new CommandRegistry();

            Assert.Throws<NotSupportedException>(() => sut.Register(invalidCommandType));
        }

        [Fact]
        public void Find_returns_expected_command_when_direct_match()
        {
            var expected = typeof(FooCommand);
            var sut = new CommandRegistry();
            sut.Register(expected);
            
            var result = sut.Find("Foo");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void does_not_throw_exception_when_same_command_is_added_multiple_times()
        {
            var sut = new CommandRegistry();
            sut.Register(typeof(FooCommand));
            sut.Register(typeof(FooCommand));
        }

        [Fact]
        public void throws_exception_if_commands_with_same_name_are_added()
        {
            var firstCommand = typeof(FakeNamespace1.DummyCommand);
            var secondCommandWithSameName = typeof(FakeNamespace2.DummyCommand);

            var sut = new CommandRegistry();
            sut.Register(firstCommand);

            Assert.Throws<Exception>(() => sut.Register(secondCommandWithSameName));
        }

        [Theory]
        [InlineData("Foo", "Foo")]
        [InlineData("FooCommand", "Foo")]
        [InlineData("FooCmd", "Foo")]
        [InlineData("FooCOMMAND", "Foo")]
        [InlineData("FooCMD", "Foo")]
        public void clean_command_names(string typeName, string expected)
        {
            var result = CommandRegistry.ExtractCommandNameFrom(typeName);
            Assert.Equal(expected, result);
        }

        #region command test doubles

        private class FooCommand : ICommand<DummyArgument>
        {
            public void Execute(DummyArgument args)
            {
                
            }
        }

        private class BarCommand : ICommand<DummyArgument>
        {
            public void Execute(DummyArgument args)
            {
                
            }
        }

        private static class FakeNamespace1
        {
            public class DummyCommand : ICommand<DummyArgument>
            {
                public void Execute(DummyArgument args)
                {

                }
            }
        }

        private static class FakeNamespace2
        {
            public class DummyCommand : ICommand<DummyArgument>
            {
                public void Execute(DummyArgument args)
                {

                }
            }
        }

        #endregion
    }

    public class CommandRegistry
    {
        private readonly List<Type> _commandTypes = new List<Type>(); 

        public Type Find(string commandName)
        {
            return _commandTypes
                .Where(x => x.Name.StartsWith(commandName))
                .SingleOrDefault();
        }

        public IEnumerable<Type> GetAll()
        {
            return _commandTypes;
        }

        public void Register(Type commandType)
        {
            var alreadyContains = Contains(commandType);
            if (alreadyContains)
            {
                return;
            }

            var isRealCommand = TypeUtils.IsAssignableToGenericType(commandType, typeof(ICommand<>));
            if (!isRealCommand)
            {
                throw new NotSupportedException(string.Format("Only types that implement {0} are allowed.", typeof(ICommand<>).FullName));
            }

            var commandName = ExtractCommandNameFrom(commandType);
            var commandNameExists = Find(commandName) != null;
            if (commandNameExists)
            {
                throw new Exception();
            }

            _commandTypes.Add(commandType);
        }

        private string ExtractCommandNameFrom(Type commandType)
        {
            return ExtractCommandNameFrom(commandType.Name);
        }

        public static string ExtractCommandNameFrom(string typeName)
        {
            return Regex.Replace(typeName, @"^(?<name>.*?)(Command|Cmd)$", "${name}", RegexOptions.IgnoreCase);
        }

        private bool Contains(Type commandType)
        {
            return _commandTypes.Contains(commandType);
        }

        public void Register(IEnumerable<Type> commandTypes)
        {
            foreach (var commandType in commandTypes)
            {
                Register(commandType);
            }
        }
    }
}