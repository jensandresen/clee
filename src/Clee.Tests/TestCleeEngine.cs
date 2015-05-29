using System;
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

        #region test data

        public struct IdArgument : ICommandArguments
        {
            public string Id { get; set; }
        }

        #endregion
    }
}