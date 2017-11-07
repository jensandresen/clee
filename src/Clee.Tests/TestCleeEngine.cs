using System;
using System.Linq;
using Clee.SystemCommands;
using Clee.Tests.Builders;
using Clee.Tests.TestDoubles;
using Clee.Tests.TestDoubles.Dummies;
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
            var engine = new CleeEngineBuilder().Build();
            Assert.IsType<DefaultCommandRegistry>(engine.Registry);
        }

        [Fact]
        public void default_engine_returns_expected_mapper()
        {
            var engine = new CleeEngineBuilder().Build();
            Assert.IsType<DefaultArgumentMapper>(engine.Mapper);
        }

        [Fact]
        public void default_engine_returns_expected_factort()
        {
            var engine = new CleeEngineBuilder().Build();
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
            var sut = new CleeEngineBuilder().Build();
            sut.Execute("");

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_1_1()
        {
            var sut = new CleeEngineBuilder().Build();
            sut.Execute((string)null);

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_2()
        {
            var sut = new CleeEngineBuilder().Build();
            sut.Execute(new string[0]);

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_2_1()
        {
            var sut = new CleeEngineBuilder().Build();
            sut.Execute((string[])null);

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_3()
        {
            var sut = new CleeEngineBuilder().Build();
            sut.Execute(
                    commandName: "",
                    args: new Argument[0]
                );

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_3_1()
        {
            var sut = new CleeEngineBuilder().Build();
            sut.Execute(
                    commandName: null,
                    args: new Argument[0]
                );

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_3_2()
        {
            var sut = new CleeEngineBuilder().Build();
            sut.Execute(
                    commandName: "",
                    args: null
                );

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void help_command_is_executed_if_no_command_is_specified_from_the_commandline_3_3()
        {
            var sut = new CleeEngineBuilder().Build();
            sut.Execute(
                    commandName: null,
                    args: null
                );

            Assert.Equal("help", sut.History.Single().CommandName);
        }

        [Fact]
        public void does_not_throw_exception_on_command_exception_when_disabled_in_settings()
        {
            var errorCommand = new ExceptionThrowingCommand();

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Settings(s => { s.ThrowOnExceptions = false; });
                cfg.Factory(f => f.Use(new StubCommandFactory(errorCommand)));
                cfg.Registry(r => r.Register("foo", errorCommand.GetType()));
            });

            engine.Execute("foo");
        }

        [Fact]
        public void throws_exception_on_command_exception_when_enabled_in_settings()
        {
            var errorCommand = new ExceptionThrowingCommand();

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Settings(s => { s.ThrowOnExceptions = true; });
                cfg.Factory(f => f.Use(new StubCommandFactory(errorCommand)));
                cfg.Registry(r => r.Register("foo", errorCommand.GetType()));
            });

            Assert.Throws<CustomCommandException>(() => engine.Execute("foo"));
        }

        [Fact]
        public void has_expected_exit_code_when_command_does_NOT_throw_exception()
        {
            var exitCode = 0;

            var errorCommand = new FooCommand();

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Factory(f => f.Use(new StubCommandFactory(errorCommand)));
                cfg.Registry(r => r.Register("foo", errorCommand.GetType()));
            });

            engine.SetExitCodeAssigner(code => exitCode = code);

            engine.Execute("foo");

            Assert.Equal(0, exitCode);
        }

        [Fact]
        public void has_expected_exit_code_when_command_throws_exception()
        {
            var exitCode = 0;

            var errorCommand = new ExceptionThrowingCommand();

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Settings(s => { s.ThrowOnExceptions = false; });
                cfg.Factory(f => f.Use(new StubCommandFactory(errorCommand)));
                cfg.Registry(r => r.Register("foo", errorCommand.GetType()));
            });

            engine.SetExitCodeAssigner(code => exitCode = code);

            engine.Execute("foo");

            Assert.Equal(-1, exitCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void has_expected_exit_code_from_command_exception(int expectedExitCode)
        {
            var exitCode = 0;

            var errorCommand = new ExceptionThrowingCommandWithExitCode(expectedExitCode);

            var engine = CleeEngine.Create(cfg =>
            {
                cfg.Settings(s => { s.ThrowOnExceptions = false; });
                cfg.Factory(f => f.Use(new StubCommandFactory(errorCommand)));
                cfg.Registry(r => r.Register("foo", errorCommand.GetType()));
            });

            engine.SetExitCodeAssigner(code => exitCode = code);

            engine.Execute("foo");

            Assert.Equal(expectedExitCode, exitCode);
        }

        #region test data

        public struct IdArgument : ICommandArguments
        {
            public string Id { get; set; }
        }

        private class ExceptionThrowingCommand : ICommand<EmptyArgument>
        {
            public void Execute(EmptyArgument args)
            {
                throw new CustomCommandException($"Error from {this.GetType().Name} command.");
            }
        }

        private class ExceptionThrowingCommandWithExitCode : ICommand<EmptyArgument>
        {
            private readonly int _exitCode;

            public ExceptionThrowingCommandWithExitCode(int exitCode)
            {
                _exitCode = exitCode;
            }

            public void Execute(EmptyArgument args)
            {
                throw new ExceptionWithExitCode(_exitCode);
            }
        }

        private class ExceptionWithExitCode : Exception, IHaveAnExitCode
        {
            public ExceptionWithExitCode(int exitCode)
            {
                ExitCode = exitCode;
            }

            public int ExitCode { get; private set; }
        }

        private class CustomCommandException : Exception
        {
            public CustomCommandException(string message) : base(message)
            {
                
            }
        }

        #endregion
    }
}