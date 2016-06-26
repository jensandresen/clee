using System;
using Moq;
using Xunit;

namespace Clee.Tests
{
    public class TestCleeEngine
    {
        [Fact]
        public void executes_expected_command_when_passed_directly()
        {
            var mock = new Mock<Command>();

            var sut = new CleeEngineBuilder().Build();
            sut.Execute(mock.Object);

            mock.Verify(x => x.Execute(), Times.Once);
        }

        [Fact]
        public void returns_expected_error_code_if_command_is_null()
        {
            var sut = new CleeEngineBuilder()
                .WithErrorHandlerEngine(new StubErrorHandlerEngine(CommandExecutionResultsType.CommandIsNull))
                .Build();

            var result = sut.Execute(null);

            Assert.Equal((int) CommandExecutionResultsType.CommandIsNull, result);
        }

        [Fact]
        public void uses_command_resolver_to_resolve_command_by_type_definition()
        {
            var dummyCommand = new CommandBuilder().Build();
            
            var resolverMock = new Mock<ICommandResolver>();
            resolverMock
                .Setup(x => x.Resolve<Command>())
                .Returns(dummyCommand);

            var sut = new CleeEngineBuilder()
                .WithCommandResolver(resolverMock.Object)
                .Build();

            sut.Execute<Command>();

            resolverMock.Verify(x => x.Resolve<Command>(), Times.Once);
        }

        [Fact]
        public void executes_the_resolved_command()
        {
            var mock = new Mock<Command>();

            var sut = new CleeEngineBuilder()
                .WithCommandResolver(new StubCommandResolver(mock.Object))
                .Build();

            sut.Execute<Command>();

            mock.Verify(x => x.Execute(), Times.Once);
        }

        [Fact]
        public void releases_the_resolved_command_after_execution()
        {
            var dummyCommand = new CommandBuilder().Build();

            var mock = new Mock<ICommandResolver>();
            mock
                .Setup(x => x.Resolve<Command>())
                .Returns(dummyCommand);

            var sut = new CleeEngineBuilder()
                .WithCommandResolver(mock.Object)
                .Build();

            sut.Execute<Command>();

            mock.Verify(x => x.Release(It.IsAny<Command>()), Times.Once);
        }

        [Fact]
        public void returns_expected_if_command_is_sucessfully_executed()
        {
            var dummy = new CommandBuilder().Build();

            var sut = new CleeEngineBuilder().Build();
            var result = sut.Execute(dummy);

            Assert.Equal((int) CommandExecutionResultsType.Ok, result);
        }

        [Fact]
        public void returns_expected_if_command_is_resolved_by_generic_argument_and_sucessfully_executed()
        {
            var dummy = new CommandBuilder().Build();

            var sut = new CleeEngineBuilder()
                .WithCommandResolver(new StubCommandResolver(dummy))
                .Build();

            var result = sut.Execute<Command>();

            Assert.Equal((int) CommandExecutionResultsType.Ok, result);
        }
    }

    public class StubErrorHandlerEngine : IErrorHandlerEngine
    {
        private readonly CommandExecutionResultsType _result;

        public StubErrorHandlerEngine(CommandExecutionResultsType result)
        {
            _result = result;
        }

        public CommandExecutionResultsType Handle(Exception error)
        {
            return _result;
        }
    }

    public class CleeEngine
    {
        private readonly ICommandResolver _commandResolver;
        private readonly IErrorHandlerEngine _errorHandlerEngine;

        public CleeEngine(ICommandResolver commandResolver, IErrorHandlerEngine errorHandlerEngine)
        {
            _commandResolver = commandResolver;
            _errorHandlerEngine = errorHandlerEngine;
        }

        private void InternalExecute(Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            command.Execute();
        }

        private bool TryExecute(Command command, out Exception error)
        {
            var success = false;

            try
            {
                InternalExecute(command);
                
                error = null;
                success = true;
            }
            catch (Exception err)
            {
                error = err;
            }

            return success;
        }

        public int Execute(Command command)
        {
            var returnCode = CommandExecutionResultsType.Ok;

            Exception exceptionThrown;

            if (!TryExecute(command, out exceptionThrown))
            {
                returnCode = _errorHandlerEngine.Handle(exceptionThrown);
            }

            return (int)returnCode;
        }
        
        public int Execute<T>() where T : Command
        {
            var command = _commandResolver.Resolve<T>();

            try
            {
                return Execute(command);
            }
            finally
            {
                _commandResolver.Release(command);
            }
        }
    }

    public interface IErrorHandlerEngine
    {
        CommandExecutionResultsType Handle(Exception error);
    }

    public interface ICommandResolver
    {
        T Resolve<T>() where T : Command;
        void Release(Command command);
    }

    public class StubCommandResolver : ICommandResolver
    {
        private readonly Command _result;

        public StubCommandResolver(Command result)
        {
            _result = result;
        }

        public T Resolve<T>() where T : Command
        {
            return (T) _result;
        }

        public void Release(Command command)
        {
            
        }
    }

    public abstract class Command
    {
        public abstract void Execute();
    }

    internal class CommandBuilder
    {
        public Command Build()
        {
            var dummy = new Mock<Command>().Object;
            return dummy;
        }
    }

    internal class CleeEngineBuilder
    {
        private ICommandResolver _commandResolver;
        private IErrorHandlerEngine _errorHandlerEngine;

        public CleeEngineBuilder()
        {
            _commandResolver = new Mock<ICommandResolver>().Object;
            _errorHandlerEngine = new Mock<IErrorHandlerEngine>().Object;
        }

        public CleeEngineBuilder WithCommandResolver(ICommandResolver commandResolver)
        {
            _commandResolver = commandResolver;
            return this;
        }

        public CleeEngineBuilder WithErrorHandlerEngine(IErrorHandlerEngine errorHandlerEngine)
        {
            _errorHandlerEngine = errorHandlerEngine;
            return this;
        }

        public CleeEngine Build()
        {
            return new CleeEngine(
                    commandResolver: _commandResolver,
                    errorHandlerEngine: _errorHandlerEngine
                );
        }
    }

    public enum CommandExecutionResultsType
    {
        Error = -1,
        Ok = 0,
        CommandIsNull
    }
}