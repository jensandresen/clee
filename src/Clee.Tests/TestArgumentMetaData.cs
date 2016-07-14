using System;
using System.Linq;
using Xunit;

namespace Clee.Tests
{
    public class TestArgumentMetaData
    {
        [Fact]
        public void returns_expected_type()
        {
            var property = typeof (SingleArgument)
                .GetProperties()
                .SingleOrDefault();

            var sut = new ArgumentMetaData(property);

            Assert.Equal(typeof(string), sut.ArgumentType);
        }

        [Fact]
        public void returns_expected_name()
        {
            var property = typeof (SingleArgument)
                .GetProperties()
                .SingleOrDefault();

            var sut = new ArgumentMetaData(property);

            Assert.Equal("foo", sut.ArgumentLongName);
        }

        [Fact]
        public void returns_expected_name_when_overridden_with_attribute()
        {
            var property = typeof (LongNameDefinedWithAttribute)
                .GetProperties()
                .SingleOrDefault();

            var sut = new ArgumentMetaData(property);

            Assert.Equal("bar", sut.ArgumentLongName);
        }

        [Fact]
        public void returns_expected_short_name()
        {
            var property = typeof(ShortNameDefinedWithAttribute)
                .GetProperties()
                .SingleOrDefault();

            var sut = new ArgumentMetaData(property);

            Assert.Equal('f', sut.ArgumentShortName);
        }

        [Fact]
        public void short_name_defaults_to_expected_value_when_not_defined_with_attribute()
        {
            var property = typeof(SingleArgument)
                .GetProperties()
                .SingleOrDefault();

            var sut = new ArgumentMetaData(property);

            Assert.Null(sut.ArgumentShortName);
        }

        [Fact]
        public void returns_expected_description()
        {
            var property = typeof(DescriptionDefinedWithAttribute)
                .GetProperties()
                .SingleOrDefault();

            var sut = new ArgumentMetaData(property);
            
            Assert.Equal("this is foo", sut.ArgumentDescription);
        }

        [Fact]
        public void returns_expected_default_description_when_not_defined_with_attribute()
        {
            var property = typeof(SingleArgument)
                .GetProperties()
                .SingleOrDefault();

            var sut = new ArgumentMetaData(property);
            
            Assert.Equal("", sut.ArgumentDescription);
        }

        [Fact]
        public void by_default_arguments_are_required()
        {
            var property = typeof(SingleArgument)
                .GetProperties()
                .SingleOrDefault();

            var sut = new ArgumentMetaData(property);
            
            Assert.True(sut.ArgumentIsRequired);
        }

        [Fact]
        public void returns_expected_when_arguments_are_not_required()
        {
            var property = typeof(NotRequiredArgumentDefinedWithAttribute)
                .GetProperties()
                .SingleOrDefault();

            var sut = new ArgumentMetaData(property);
            
            Assert.False(sut.ArgumentIsRequired);
        }

        [Fact]
        public void ctor_throws_exception_if_initialized_with_null()
        {
            Assert.Throws<ArgumentNullException>(() => new ArgumentMetaData(null));
        }

        [Fact]
        public void ctor_throws_exception_if_initialized_with_property_without_the_proper_attribute()
        {
            var property = typeof(NoArgumentAttribute)
                .GetProperties()
                .SingleOrDefault();

            Assert.Throws<ArgumentException>(() => new ArgumentMetaData(property));
        }

        #region dummy classes

        private class NoArgumentAttribute
        {
            public string Foo { get; set; }
        }

        private class SingleArgument
        {
            [Argument]
            public string Foo { get; set; }
        }

        private class LongNameDefinedWithAttribute
        {
            [Argument(LongName = "bar")]
            public string Foo { get; set; }
        }

        private class ShortNameDefinedWithAttribute
        {
            [Argument(ShortName = 'f')]
            public string Foo { get; set; }
        }

        private class DescriptionDefinedWithAttribute
        {
            [Argument(Description = "this is foo")]
            public string Foo { get; set; }
        }

        private class NotRequiredArgumentDefinedWithAttribute
        {
            [Argument(IsRequired = false)]
            public string Foo { get; set; }
        }

        #endregion
    }
}