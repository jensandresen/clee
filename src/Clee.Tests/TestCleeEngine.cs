using System;
using System.Linq;
using Clee.SystemCommands;
using Clee.Tests.TestDoubles;
using Moq;
using Xunit;

namespace Clee.Tests
{
    public class TestCleeEngine
    {
        #region structural inspection

        [Fact]
        public void returns_expected_factory()
        {
            var expected = new Mock<ICommandFactory>().Object;

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Factory(f => f.Use(expected));
            });

            Assert.Same(expected, engine.Factory);
        }

        [Fact]
        public void returns_expected_mapper()
        {
            var expected = new Mock<IArgumentMapper>().Object;

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Mapper(m => m.Use(expected));
            });

            Assert.Same(expected, engine.Mapper);
        }

        [Fact]
        public void returns_expected_registry()
        {
            var expected = new Mock<ICommandRegistry>().Object;

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Registry(r => r.Use(expected));
            });

            Assert.Same(expected, engine.Registry);
        }

        [Fact]
        public void default_engine_returns_expected_registry()
        {
            var engine = CleeEngine.CreateDefault();
            Assert.IsType<DefaultCommandRegistry>(engine.Registry);
        }

        [Fact]
        public void default_engine_returns_expected_mapper()
        {
            var engine = CleeEngine.CreateDefault();
            Assert.IsType<DefaultArgumentMapper>(engine.Mapper);
        }

        [Fact]
        public void default_engine_returns_expected_factort()
        {
            var engine = CleeEngine.CreateDefault();
            Assert.IsType<DefaultCommandFactory>(engine.Factory);
        }

        #endregion

        [Fact]
        public void can_executed_expected_command_from_single_input_line()
        {
            var mock = new Mock<ICommand<EmptyArgument>>();

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Factory(f => f.Use(new StubCommandFactory(mock.Object)));
                cfg.Registry(r => r.Register("foo", mock.Object.GetType()));
            });

            engine.Execute("foo");

            mock.Verify(x => x.Execute(It.IsAny<EmptyArgument>()));
        }

        [Fact]
        public void can_executed_expected_command_from_list_of_inputs()
        {
            var mock = new Mock<ICommand<EmptyArgument>>();

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Factory(f => f.Use(new StubCommandFactory(mock.Object)));
                cfg.Registry(r => r.Register("foo", mock.Object.GetType()));
            });

            engine.Execute(new[] { "foo" });

            mock.Verify(x => x.Execute(It.IsAny<EmptyArgument>()));
        }

        [Fact]
        public void executed_command_with_expected_argument()
        {
            var mock = new Mock<ICommand<IdArgument>>();

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Factory(f => f.Use(new StubCommandFactory(mock.Object)));
                cfg.Registry(r => r.Register("foo", mock.Object.GetType()));
            });

            engine.Execute("foo -id 1");

            var expectedArgument = new IdArgument { Id = "1" };

            mock.Verify(x => x.Execute(expectedArgument));
        }

        [Fact]
        public void throws_exception_if_required_argument_is_not_specified()
        {
            var stubCommand = new Mock<ICommand<IdArgument>>().Object;

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Factory(f => f.Use(new StubCommandFactory(stubCommand)));
                cfg.Registry(r => r.Register("foo", stubCommand.GetType()));
            });

            Assert.Throws<Exception>(() => engine.Execute("foo"));
        }

        [Fact]
        public void throws_exception_when_executing_a_command_that_has_not_been_registered()
        {
            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Registry(r => { /* no command registration! */});
            });

            Assert.Throws<NotSupportedException>(() => engine.Execute("foo"));
        }

        [Fact]
        public void system_commands_are_not_released_using_custom_factory()
        {
            var mock = new Mock<ICommandFactory>();

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Factory(f => f.Use(mock.Object));
            });

            engine.Execute("help");

            mock.Verify(x => x.Release(It.IsAny<HelpCommand>()), Times.Never());
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_1()
        {
            var sut = CleeEngine.CreateDefault();
            sut.Execute("");

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_1_1()
        {
            var sut = CleeEngine.CreateDefault();
            sut.Execute((string) null);

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_2()
        {
            var sut = CleeEngine.CreateDefault();
            sut.Execute(new string[0]);

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_2_1()
        {
            var sut = CleeEngine.CreateDefault();
            sut.Execute((string[]) null);

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_3()
        {
            var sut = CleeEngine.CreateDefault();
            sut.Execute(
                    commandName: "",
                    args: new Argument[0]
                );

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_3_1()
        {
            var sut = CleeEngine.CreateDefault();
            sut.Execute(
                    commandName: null,
                    args: new Argument[0]
                );

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_3_2()
        {
            var sut = CleeEngine.CreateDefault();
            sut.Execute(
                    commandName: "",
                    args: null
                );

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_3_3()
        {
            var sut = CleeEngine.CreateDefault();
            sut.Execute(
                    commandName: null,
                    args: null
                );

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        #region test data

        public struct IdArgument : ICommandArguments
        {
            public string Id { get; set; }
        }

        #endregion
    }
}