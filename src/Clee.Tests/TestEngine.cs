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

            engine.Execute("foo");
        }

        private class FooData : ICommandArguments
        {
            [Value(IsOptional = true)]
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