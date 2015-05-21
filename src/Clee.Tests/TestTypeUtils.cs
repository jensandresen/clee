using System;
using Clee.Types;
using Xunit;

namespace Clee.Tests
{
    public class TestTypeUtils
    {
        [Fact]
        public void can_extract_argument_type_from_command_that_targets_single_argument()
        {
            var result = TypeUtils.ExtractArgumentTypesFromCommand(new SingleArgumentCommand());
            var expected = new[] {typeof (FooArgument)};

            Assert.Equal(expected, result);
        }

        [Fact]
        public void can_extract_argument_type_from_command_that_targets_multiple_arguments()
        {
            var result = TypeUtils.ExtractArgumentTypesFromCommand(new MultiArgumentCommand());
            
            var expected = new[]
            {
                typeof (FooArgument),
                typeof (BarArgument),
            };

            Assert.Equal(expected, result);
        }

        [Fact]
        public void can_extract_command_implementations_of_a_type()
        {
            var result = TypeUtils.ExtractCommandImplementationsFromType(typeof (MultiArgumentCommand));

            var expected = new[]
            {
                typeof (ICommand<FooArgument>),
                typeof (ICommand<BarArgument>),
            };

            Assert.Equal(expected, result);
        }


        private class FooArgument : ICommandArguments { }
        private class BarArgument : ICommandArguments { }

        private class SingleArgumentCommand : ICommand<FooArgument>
        {
            public void Execute(FooArgument args) { }
        }

        private class MultiArgumentCommand : ICommand<FooArgument>, ICommand<BarArgument>
        {
            public void Execute(FooArgument args) { }
            public void Execute(BarArgument args) { }
        }
    }
}