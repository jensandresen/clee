using System;
using Moq;
using Xunit;

namespace Clee.Tests
{
    public class TestDefaultCommandExecutor
    {
        [Fact]
        public void invokes_execute_method()
        {
            var mock = new Mock<ICommand<EmptyArgument>>();
            var dummyArgument = new EmptyArgument();
            var sut = new DefaultCommandExecutor();

            sut.Execute(mock.Object, dummyArgument);

            mock.Verify(x => x.Execute(dummyArgument));
        }

        [Fact]
        public void throws_exception_if_command_is_invalid_type()
        {
            var invalidCommand = "string is not a valid command type";
            var dummyArgument = new EmptyArgument();

            var sut = new DefaultCommandExecutor();

            Assert.Throws<NotSupportedException>(() => sut.Execute(invalidCommand, dummyArgument));
        }

        [Fact]
        public void throws_exception_if_argument_is_invalid_type()
        {
            var dummyCommand = new Mock<ICommand<EmptyArgument>>().Object;
            var invalidArgument = "string is not a valid command type";

            var sut = new DefaultCommandExecutor();

            Assert.Throws<NotSupportedException>(() => sut.Execute(dummyCommand, invalidArgument));
        }

        [Fact]
        public void invokes_expected_method_when_overloads_exists()
        {
            var spy = new SpyOverloadExecuteMethodCommand();

            var sut = new DefaultCommandExecutor();
            sut.Execute(spy, new EmptyArgument());

            Assert.True(spy.wasExpectedInvoked);
        }

        private class SpyOverloadExecuteMethodCommand : ICommand<EmptyArgument>
        {
            public bool wasExpectedInvoked = false;

            public void Execute()
            {

            }

            public void Execute(EmptyArgument args)
            {
                wasExpectedInvoked = true;
            }

            public void Execute(string args)
            {
                
            }

            public void Execute(EmptyArgument args, EmptyArgument moreArgs)
            {
                
            }
        }
    }
}