using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Clee.Tests
{
    public class TestCommandMetaData
    {
        [Theory]
        [InlineData(typeof(DummyCommand))]
        [InlineData(typeof(NakedAndEmptyCommand))]
        [InlineData(typeof(NameDefinedWithAttributeCommand))]
        public void returns_expected_command_type_from_type(Type commandType)
        {
            var metaData = new CommandMetaData(commandType);
            Assert.Equal(commandType, metaData.CommandType);
        }

        [Theory]
        [InlineData(typeof(DummyCommand), "dummy")]
        [InlineData(typeof(NakedAndEmptyCommand), "nakedandempty")]
        public void returns_expected_command_name_from_command_type_without_attribute(Type commandType, string expectedName)
        {
            var metaData = new CommandMetaData(commandType);
            Assert.Equal(expectedName, metaData.CommandName);
        }

        [Theory]
        [InlineData(typeof(NameDefinedWithAttributeCommand), "supportsattributes")]
        public void returns_expected_command_name_from_command_type_with_attribute(Type commandType, string expectedName)
        {
            var metaData = new CommandMetaData(commandType);
            Assert.Equal(expectedName, metaData.CommandName);
        }

        [Theory]
        [InlineData(typeof(DummyCommand), "")]
        [InlineData(typeof(NakedAndEmptyCommand), "")]
        public void returns_expected_command_description_from_command_type_without_attribute(Type commandType, string expectedDescription)
        {
            var metaData = new CommandMetaData(commandType);
            Assert.Equal(expectedDescription, metaData.CommandDescription);
        }

        [Theory]
        [InlineData(typeof(DescriptionDefinedWithAttributeCommand), "supports attributes")]
        public void returns_expected_command_description_from_command_type_with_attribute(Type commandType, string expectedDescription)
        {
            var metaData = new CommandMetaData(commandType);
            Assert.Equal(expectedDescription, metaData.CommandDescription);
        }

        [Fact]
        public void returns_expected_arguments_from_command_type_with_no_arguments()
        {
            var metaData = new CommandMetaData(typeof(NakedAndEmptyCommand));
            Assert.Empty(metaData.Arguments);
        }

        [Fact]
        public void returns_expected_arguments_from_command_type_with_no_properties_marked_as_arguments()
        {
            var metaData = new CommandMetaData(typeof(SinglePropertyNotMarkedAsArgumentCommand));
            Assert.Empty(metaData.Arguments);
        }

        [Fact]
        public void returns_expected_arguments_from_command_type_with_single_argument()
        {
            var metaData = new CommandMetaData(typeof(SingleArgumentCommand));
            Assert.Single(metaData.Arguments);
        }

        [Fact]
        public void returns_expected_arguments_from_command_type_with_two_argument()
        {
            var metaData = new CommandMetaData(typeof(TwoArgumentCommand));
            Assert.Equal(2, metaData.Arguments.Count());
        }

        [Fact]
        public void ctor_throws_exception_if_initialized_with_inproper_type()
        {
            Assert.Throws<ArgumentException>(() => new CommandMetaData(typeof (string)));
        }

        [Fact]
        public void ctor_throws_exception_if_initialized_with_null()
        {
            Assert.Throws<ArgumentNullException>(() => new CommandMetaData(null));
        }

        #region dummy classes

        private class NakedAndEmptyCommand : DummyCommand { }

        [Command(Name = "supportsattributes")]
        private class NameDefinedWithAttributeCommand : DummyCommand { }

        [Command(Description = "supports attributes")]
        private class DescriptionDefinedWithAttributeCommand : DummyCommand { }

        private class SinglePropertyNotMarkedAsArgumentCommand : DummyCommand
        {
            public string Foo { get; set; }
        }

        private class SingleArgumentCommand : DummyCommand
        {
            [Argument]
            public string Foo { get; set; }
        }

        private class TwoArgumentCommand : DummyCommand
        {
            [Argument]
            public string Foo { get; set; }

            [Argument]
            public string Bar { get; set; }
        }

        #endregion
    }
}