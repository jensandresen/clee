using System;
using System.Linq;
using Clee.Tests.TestDoubles;
using Xunit;

namespace Clee.Tests
{
    public class TestCommandRegistry
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
            var firstCommand = typeof(FakeNamespace1.DummyCommand);
            var secondCommandWithSameName = typeof(FakeNamespace2.DummyCommand);

            var sut = new DefaultCommandRegistry();
            sut.Register(firstCommand);

            Assert.Throws<Exception>(() => sut.Register(secondCommandWithSameName));
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
}