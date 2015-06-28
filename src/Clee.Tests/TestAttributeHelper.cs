using System;
using Xunit;

namespace Clee.Tests
{
    public class TestAttributeHelper
    {
        [Fact]
        public void can_extract_method_description()
        {
            var result = AttributeHelper.GetDescription(typeof (TestCommand), typeof (ICommand<FooArgument>));
            Assert.Equal("foo", result);
        }

        [Fact]
        public void defaults_to_class_description_if_method_is_not_annotated()
        {
            var result = AttributeHelper.GetDescription(typeof (TestCommand), typeof (ICommand<BarArgument>));
            Assert.Equal("bar", result);
        }

        [Fact]
        public void defaults_to_full_type_name_if_method_or_class_is_not_annotated()
        {
            var result = AttributeHelper.GetDescription(typeof (NoDescriptionCommand), typeof (ICommand<EmptyArgument>));
            Assert.Equal(typeof(NoDescriptionCommand).FullName, result);
        }

        [Fact]
        public void throws_exception_if_implementation_does_not_implement_expected_command_interface()
        {
            Assert.Throws<ArgumentException>(() => AttributeHelper.GetDescription(typeof(TestCommand), typeof(ICommand<EmptyArgument>)));
        }


        [Command(Description = "bar")]
        private class TestCommand : ICommand<FooArgument>, ICommand<BarArgument>
        {
            [Command(Description = "foo")]
            public void Execute(FooArgument args)
            {
                
            }

            public void Execute(BarArgument args)
            {
                
            }
        }

        private class FooArgument : ICommandArguments { }
        private class BarArgument : ICommandArguments { }

        private class NoDescriptionCommand : ICommand<EmptyArgument>
        {
            public void Execute(EmptyArgument args)
            {
                
            }
        }
    }
}