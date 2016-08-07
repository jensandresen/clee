using Clee.Routing;
using Clee.Tests.TestDoubles;
using Xunit;

namespace Clee.Tests
{
    public class TestNameAsPathStrategy
    {
        [Fact]
        public void is_command_path_strategy()
        {
            var sut = new NameAsPathStrategy();
            Assert.IsAssignableFrom<ICommandPathStrategy>(sut);
        }

        [Fact]
        public void returns_expected_path_from_command()
        {
            var sut = new NameAsPathStrategy();
            var result = sut.GeneratePathFor(new CommandMetaData(typeof (DummyCommand)));

            Assert.Equal(new Path("dummy"), result);
        }
    }
}