using Clee.Routing;

namespace Clee.Tests.TestDoubles
{
    public class StubCommandPathStrategy : ICommandPathStrategy
    {
        private readonly Path _result;

        public StubCommandPathStrategy(Path result)
        {
            _result = result;
        }

        public Path GeneratePathFor(CommandMetaData metaData)
        {
            return _result;
        }
    }
}