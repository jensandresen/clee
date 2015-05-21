using Xunit;

namespace Clee.Tests
{
    public class TestEngine
    {
        [Fact]
        public void testname()
        {
            var registry = new DefaultCommandRegistry();
            registry.Register(typeof(FooCommand));

            var engine = Engine.Create(cfg =>
            {
                cfg.WithRegistry(registry);
            });

//            engine.Execute("foo -name bar");
            engine.Execute("--list");
        }

        private class FooData : ICommandArguments
        {
            public string Name { get; set; }
        }

        private class FooCommand : ICommand<FooData>
        {
            public void Execute(FooData args)
            {
                
            }
        }
    }
}