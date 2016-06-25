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
        public void throws_exception_if_command_is_null()
        {
            var sut = new CleeEngineBuilder().Build();
            Assert.Throws<Exception>(() => sut.Execute(null));
        }

        [Fact]
        public void uses_command_resolver_to_resolve_command_by_type_definition()
        {
            var dummyCommand = new Mock<Command>().Object;
            
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
    }

    public class CleeEngine
    {
        private readonly ICommandResolver _commandResolver;

        public CleeEngine(ICommandResolver commandResolver)
        {
            _commandResolver = commandResolver;
        }

        public void Execute(Command command)
        {
            if (command == null)
            {
                throw new Exception();
            }

            command.Execute();
        }

        public void Execute<T>() where T : Command
        {
            var command = _commandResolver.Resolve<T>();
            Execute(command);
        }
    }

    public interface ICommandResolver
    {
        T Resolve<T>() where T : Command;
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
    }

    public abstract class Command
    {
        public abstract void Execute();
    }

    internal class CleeEngineBuilder
    {
        private ICommandResolver _commandResolver;

        public CleeEngineBuilder()
        {
            _commandResolver = new Mock<ICommandResolver>().Object;
        }

        public CleeEngineBuilder WithCommandResolver(ICommandResolver commandResolver)
        {
            _commandResolver = commandResolver;
            return this;
        }

        public CleeEngine Build()
        {
            return new CleeEngine(
                    commandResolver: _commandResolver
                );
        }
    }
}