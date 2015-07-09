using System;
using System.Linq;
using Clee.Tests.TestDoubles.Dummies;
using Xunit;

namespace Clee.Tests
{
    public class TestDefaultCommandRegistry
    {
        [Fact]
        public void Find_returns_expected_when_no_commands_are_registered()
        {
            var sut = new DefaultCommandRegistry();
            var result = sut.Find("foo");

            Assert.Null(result);
        }

        [Fact]
        public void GetAll_returns_expected_when_empty()
        {
            var sut = new DefaultCommandRegistry();
            var result = sut.GetAll();

            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_returns_expected_when_single_command_is_added()
        {
            var sut = new DefaultCommandRegistry();
            sut.Register(typeof (FooCommand));

            var result = sut
                .GetAll()
                .Select(x => x.ImplementationType)
                .ToArray();

            Assert.Equal(new[]{typeof(FooCommand)}, result);
        }

        [Fact]
        public void GetAll_returns_expected_when_multiple_commands_are_added()
        {
            var sut = new DefaultCommandRegistry();
            sut.Register(typeof (FooCommand));
            sut.Register(typeof (BarCommand));
            
            var result = sut
                .GetAll()
                .Select(x => x.ImplementationType)
                .ToArray();

            Assert.Equal(new[]
            {
                typeof(FooCommand),
                typeof(BarCommand),
            }, result);
        }

        [Fact]
        public void can_add_multiple_commands_at_once()
        {
            var sut = new DefaultCommandRegistry();
            sut.Register(new[]
            {
                typeof(FooCommand),
                typeof(BarCommand),
            });
            
            var result = sut
                .GetAll()
                .Select(x => x.ImplementationType)
                .ToArray();

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
            var sut = new DefaultCommandRegistry();

            Assert.Throws<NotSupportedException>(() => sut.Register(invalidCommandType));
        }

        [Fact]
        public void Find_returns_expected_command_when_direct_match()
        {
            var expected = typeof(FooCommand);
            var sut = new DefaultCommandRegistry();
            sut.Register(expected);
            
            var result = sut.Find("Foo");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Find_is_case_insensitive()
        {
            var expected = typeof(FooCommand);
            var sut = new DefaultCommandRegistry();
            sut.Register(expected);
            
            var result = sut.Find("foo");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void does_not_throw_exception_when_same_command_is_added_multiple_times()
        {
            var sut = new DefaultCommandRegistry();
            sut.Register(typeof(FooCommand));
            sut.Register(typeof(FooCommand));
        }

        [Fact]
        public void throws_exception_if_commands_with_same_name_are_added()
        {
            var firstCommand = typeof(FooCommand);
            var secondCommand = typeof(BarCommand);

            var sut = new DefaultCommandRegistry();
            sut.Register("foo", firstCommand);

            Assert.Throws<Exception>(() => sut.Register("foo", secondCommand));
        }

        [Fact]
        public void Register_returns_expected_command_registration()
        {
            var dummyCommand = typeof(FooCommand);

            var sut = new DefaultCommandRegistry();
            var result = sut.Register(dummyCommand);

            var expected = new CommandRegistration(
                commandName: "foo",
                commandType: typeof (ICommand<EmptyArgument>),
                argumentType: typeof (EmptyArgument),
                implementationType: typeof (FooCommand)
                );

            Assert.Equal(expected, result);
        }

        [Fact]
        public void can_use_specific_command_name()
        {
            var expected = new CommandRegistration(
                commandName: "anoterCommandName",
                commandType: typeof(ICommand<EmptyArgument>),
                argumentType: typeof(EmptyArgument),
                implementationType: typeof(FooCommand)
                );

            var sut = new DefaultCommandRegistry();
            var result = sut.Register(expected.CommandName, expected.ImplementationType);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Foo", "foo")]
        [InlineData("FooCommand", "foo")]
        [InlineData("FooCmd", "foo")]
        [InlineData("FooCOMMAND", "foo")]
        [InlineData("FooCMD", "foo")]
        public void clean_command_names(string typeName, string expected)
        {
            var result = DefaultCommandRegistry.ExtractCommandNameFrom(typeName);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void throws_exception_if_registering_a_type_with_multiple_command_implementations()
        {
            var sut = new DefaultCommandRegistry();
            Assert.Throws<NotSupportedException>(() => sut.Register(typeof (MultiArgumentCommand)));
        }

        [Fact]
        public void throws_exception_if_registering_a_type_with_multiple_command_implementations_2()
        {
            var sut = new DefaultCommandRegistry();
            Assert.Throws<NotSupportedException>(() => sut.Register(new[] {typeof (MultiArgumentCommand)}));
        }

        [Fact]
        public void throws_exception_if_registering_a_type_with_multiple_command_implementations_3()
        {
            var sut = new DefaultCommandRegistry();
            Assert.Throws<NotSupportedException>(() => sut.Register("dummyName", typeof(MultiArgumentCommand)));
        }

        [Fact]
        public void favor_command_name_from_method_annotation()
        {
            var sut = new DefaultCommandRegistry();
            sut.Register(typeof (NamedByMethodAnnotationCommand));

            var result = sut.GetAll().Single();

            Assert.Equal("foo", result.CommandName);
        }

        [Fact]
        public void favor_command_name_from_class_annotation()
        {
            var sut = new DefaultCommandRegistry();
            sut.Register(typeof (NamedByClassAnnotationCommand));

            var result = sut.GetAll().Single();

            Assert.Equal("foo", result.CommandName);
        }

        [Theory]
        [InlineData(typeof(FooCommand), "foo")]
        [InlineData(typeof(BarCommand), "bar")]
        [InlineData(typeof(SingleArgumentCommand), "singleargument")]
        public void registers_with_expected_command_name(Type commandType, string expectedCommandName)
        {
            var sut = new DefaultCommandRegistry();
            sut.Register(commandType);

            var result = sut.GetAll().Single();

            Assert.Equal(expectedCommandName, result.CommandName);
        }

        [Fact]
        public void can_use_custom_command_name_convention()
        {
            // arrange
            var expectedCommandName = "another command name";

            var sut = new DefaultCommandRegistry();
            sut.ChangeCommandNameConvention(new StubCommandNameConvention(expectedCommandName));
            
            // act
            var result = sut.Register(typeof (FooCommand));

            // assert
            Assert.Equal(expectedCommandName, result.CommandName);
        }

        #region command test doubles

        private class NamedByMethodAnnotationCommand : ICommand<EmptyArgument>
        {
            [Command(Name = "foo")]
            public void Execute(EmptyArgument args)
            {
                
            }
        }

        [Command(Name = "foo")]
        private class NamedByClassAnnotationCommand : ICommand<EmptyArgument>
        {
            public void Execute(EmptyArgument args)
            {
                
            }
        }

        #endregion
    }

    internal class StubCommandNameConvention : CommandNameConvention
    {
        private readonly string _result;

        public StubCommandNameConvention(string result)
        {
            _result = result;
        }

        public override string ExtractCommandNameFrom(string typeName)
        {
            return _result;
        }
    }
}