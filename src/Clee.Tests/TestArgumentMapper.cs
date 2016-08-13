using Clee.Mapping;
using Xunit;

namespace Clee.Tests
{
    public class TestArgumentMapper
    {
        [Fact]
        public void assigns_expected_value_on_single_command_argument_property()
        {
            var cmd = new SingleArgumentCommand();
            var sut = new ArgumentMapper();

            sut.Map(cmd, new[] {Argument.CreateLongNamed("Foo", "Bar")});

            Assert.Equal("Bar", cmd.Foo);
        }

        [Fact]
        public void assigns_expected_value_on_multiple_command_argument_properties()
        {
            var cmd = new DoubleArgumentCommand();
            var sut = new ArgumentMapper();

            sut.Map(cmd, new[]
            {
                Argument.CreateLongNamed("Foo", "Foo-Value"),
                Argument.CreateLongNamed("Bar", "Bar-Value"),
            });

            Assert.Equal("Foo-Value", cmd.Foo);
            Assert.Equal("Bar-Value", cmd.Bar);
        }

        [Fact]
        public void is_case_insensitive_on_argument_name()
        {
            var cmd = new SingleArgumentCommand();
            var sut = new ArgumentMapper();

            sut.Map(cmd, new[] { Argument.CreateLongNamed("foo", "bar") });

            Assert.Equal("bar", cmd.Foo);
        }

        [Fact]
        public void ignores_properties_that_are_not_marked_with_attribute()
        {
            var cmd = new SingleArgumentWithOtberPropertiesCommand();
            var sut = new ArgumentMapper();

            sut.Map(cmd, new[] { Argument.CreateLongNamed("foo", "foo-value") });

            Assert.Null(cmd.Bar);
        }

        [Fact]
        public void supports_argument_name_overridden_by_attribute()
        {
            var cmd = new SingleArgumentCommandWithNameOverridenByAttribute();
            var sut = new ArgumentMapper();

            sut.Map(cmd, new[] { Argument.CreateLongNamed("NameFromAttribute", "foo-value") });

            Assert.Equal("foo-value", cmd.Foo);
        }

        [Fact]
        public void supports_short_argument_name_defined_by_attribute()
        {
            var cmd = new SingleArgumentCommandWithShortNameDefinedByAttribute();
            var sut = new ArgumentMapper();

            sut.Map(cmd, new[] { Argument.CreateShortNamed("f", "foo-value") });

            Assert.Equal("foo-value", cmd.Foo);
        }

        [Fact]
        public void throws_exception_if_required_command_argument_is_not_present_amoungst_arguments_to_be_mapped()
        {
            var cmd = new SingleArgumentCommand();
            var emptyArgumentList = new Argument[0];
            
            var sut = new ArgumentMapper();

            Assert.Throws<RequiredArgumentMissingException>(() => sut.Map(cmd, emptyArgumentList));
        }

        [Fact]
        public void non_required_arguments_does_not_throw_exception_if_they_are_missing_from_arguments_to_be_mapped()
        {
            var cmd = new SingleNonRequiredArgumentCommand();
            var emptyArgumentList = new Argument[0];
            var sut = new ArgumentMapper();

            sut.Map(cmd, emptyArgumentList);

            Assert.Null(cmd.Foo);
        }

        [Fact]
        public void non_required_arguments_are_assigned_expected_value_when_available()
        {
            var cmd = new SingleNonRequiredArgumentCommand();
            var sut = new ArgumentMapper();

            sut.Map(cmd, new[] { Argument.CreateLongNamed("Foo", "foo-value") });

            Assert.Equal("foo-value", cmd.Foo);
        }

        [Fact]
        public void throws_exception_if_argunments_to_be_mapped_contains_arguments_that_are_not_defined_on_the_command()
        {
            var cmd = new SingleArgumentCommand();
            var sut = new ArgumentMapper();

            var argumentsToBeMapped = new[]
            {
                Argument.CreateLongNamed("Foo", "foo-value"),
                Argument.CreateLongNamed("Bar", "bar-value"),
            };

            Assert.Throws<UnknownCommandArgumentsException>(() => sut.Map(cmd, argumentsToBeMapped));
        }

        [Fact]
        public void throws_exception_if_there_is_no_write_access_to_a_command_argument_property()
        {
            var cmd = new SingleArgumentWithNoPublicSetterCommand();
            var sut = new ArgumentMapper();

            var argumentsToBeMapped = new[]
            {
                Argument.CreateLongNamed("Foo", "foo-value"),
            };

            Assert.Throws<UnavailableWriteAccessToCommandPropertyException>(() => sut.Map(cmd, argumentsToBeMapped));
        }

        [Fact]
        public void is_case_sensitive_on_short_name_argument_flags()
        {
            var cmd = new SingleNonRequiredArgumentCommandWithShortNameDefinedByAttribute();
            var sut = new ArgumentMapper();

            try
            {
                sut.Map(cmd, new[] {Argument.CreateShortNamed("F", "foo-value")});
            }
            catch (UnknownCommandArgumentsException)
            {
                
            }

            Assert.Null(cmd.Foo);
        }

        [Fact]
        public void throws_exception_if_an_argument_is_given_as_input_multiple_times_with_long_name()
        {
            var cmd = new SingleArgumentCommand();
            var sut = new ArgumentMapper();

            var argumentsToMap = new[]
            {
                Argument.CreateLongNamed("Foo", "foo-value"), 
                Argument.CreateLongNamed("Foo", "foo-value"), 
            };

            Assert.Throws<ArgumentDeclaredMultipleTimesException>(() => sut.Map(cmd, argumentsToMap));
        }

        [Fact]
        public void supports_flags_with_same_letter_but_different_casing()
        {
            var cmd = new DoubleArgumentCommandWithShortNamesDefinedByAttributes();
            var sut = new ArgumentMapper();

            sut.Map(cmd, new[]
            {
                Argument.CreateShortNamed("f", "foo1-value"),
                Argument.CreateShortNamed("F", "foo2-value")
            });

            Assert.Equal("foo1-value", cmd.Foo1);
            Assert.Equal("foo2-value", cmd.Foo2);
        }

        [Fact]
        public void throws_exception_if_an_argument_is_given_as_input_multiple_times_with_short_name()
        {
            var cmd = new SingleArgumentCommand();
            var sut = new ArgumentMapper();

            var argumentsToMap = new[]
            {
                Argument.CreateShortNamed("f", "foo-value"), 
                Argument.CreateShortNamed("f", "foo-value"), 
            };
            
            Assert.Throws<ArgumentDeclaredMultipleTimesException>(() => sut.Map(cmd, argumentsToMap));
        }

        [Fact]
        public void throws_exception_if_an_argument_is_given_as_input_multiple_times_with_long_and_short_name()
        {
            var cmd = new SingleArgumentCommandWithShortNameDefinedByAttribute();
            var sut = new ArgumentMapper();

            var argumentsToMap = new[]
            {
                Argument.CreateShortNamed("f", "foo-value"), 
                Argument.CreateShortNamed("foo", "foo-value"), 
            };

            Assert.Throws<ArgumentDeclaredMultipleTimesException>(() => sut.Map(cmd, argumentsToMap));
        }

        [Fact]
        public void throws_exception_if_command_has_ambigous_argument_matches_on_long_name()
        {
            var cmd = new CommandWithAmbigousArgumentDefinitionsByLongName();
            var sut = new ArgumentMapper();

            var argumentsToMap = new[]
            {
                Argument.CreateShortNamed("foo", "foo-value"), 
            };

            Assert.Throws<CommandHasAmbigousArgumentDefinitionException>(() => sut.Map(cmd, argumentsToMap));
        }

        [Fact]
        public void throws_exception_if_command_has_ambigous_argument_matches_on_short_name()
        {
            var cmd = new CommandWithAmbigousArgumentDefinitionsByShortName();
            var sut = new ArgumentMapper();

            var argumentsToMap = new[]
            {
                Argument.CreateShortNamed("f", "foo-value"), 
            };

            Assert.Throws<CommandHasAmbigousArgumentDefinitionException>(() => sut.Map(cmd, argumentsToMap));
        }

        #region dummy classes

        private class SingleArgumentCommand : Command
        {
            [Argument]
            public string Foo { get; set; }

            public override void Execute()
            {
                
            }
        }

        private class SingleNonRequiredArgumentCommand : Command
        {
            [Argument(IsRequired = false)]
            public string Foo { get; set; }

            public override void Execute()
            {
                
            }
        }

        private class SingleArgumentWithNoPublicSetterCommand : Command
        {
            [Argument]
            public string Foo { get; private set; }

            public override void Execute()
            {

            }
        }

        private class DoubleArgumentCommand : Command
        {
            [Argument]
            public string Foo { get; set; }

            [Argument]
            public string Bar { get; set; }

            public override void Execute()
            {
                
            }
        }

        private class SingleArgumentWithOtberPropertiesCommand : Command
        {
            [Argument]
            public string Foo { get; set; }

            public string Bar { get; set; }

            public override void Execute()
            {
                
            }
        }

        private class SingleArgumentCommandWithNameOverridenByAttribute : Command
        {
            [Argument(LongName = "NameFromAttribute")]
            public string Foo { get; set; }

            public override void Execute()
            {
                
            }
        }

        private class SingleArgumentCommandWithShortNameDefinedByAttribute : Command
        {
            [Argument(ShortName = 'f')]
            public string Foo { get; set; }

            public override void Execute()
            {
                
            }
        }

        private class SingleNonRequiredArgumentCommandWithShortNameDefinedByAttribute : Command
        {
            [Argument(ShortName = 'f', IsRequired = false)]
            public string Foo { get; set; }

            public override void Execute()
            {
                
            }
        }

        private class DoubleArgumentCommandWithShortNamesDefinedByAttributes : Command
        {
            [Argument(ShortName = 'f')]
            public string Foo1 { get; set; }

            [Argument(ShortName = 'F')]
            public string Foo2 { get; set; }

            public override void Execute()
            {
                
            }
        }

        private class CommandWithAmbigousArgumentDefinitionsByLongName : Command
        {
            [Argument]
            public string Foo { get; set; }

            [Argument(LongName = "foo")]
            public string Bar { get; set; }

            public override void Execute()
            {
                
            }
        }

        private class CommandWithAmbigousArgumentDefinitionsByShortName : Command
        {
            [Argument(ShortName = 'f')]
            public string Foo { get; set; }

            [Argument(ShortName = 'f')]
            public string Bar { get; set; }

            public override void Execute()
            {
                
            }
        }

        #endregion
    }
}