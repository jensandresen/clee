using Xunit;

namespace Clee.Tests
{
    public class TestEngine
    {
        [Fact]
        public void testname()
        {
            var engine = Engine.Create(cfg =>
            {
                cfg.Registry(r =>
                {
                    r.Register(typeof (FooCommand));
                });
            });

            engine.Execute("foo -name bar");
        }

        [Fact]
        public void testname2()
        {
            var engine = Engine.Create(cfg =>
            {
                cfg.Registry(r =>
                {
                    r.Register(typeof(FooCommand));
                });
            });

            engine.Execute(new[]
            {
                "foo",
                "-name",
                "bar",
            });
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