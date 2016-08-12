using Clee.Tests.Builders;
using Clee.Tests.TestDoubles;
using Clee.TypeResolving;
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
            var sut = new CleeEngineBuilder().Build();
            var result = sut.Execute((Command) null);

            Assert.Equal((int) CommandExecutionResultsType.Error, result);
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
}