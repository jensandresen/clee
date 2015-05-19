using Clee.Types;
using Xunit;
using AssemblyBuilder = Clee.Tests.Builders.AssemblyBuilder;

namespace Clee.Tests
{
    public class TestCommandScanner
    {
        [Fact]
        public void returns_expected_when_no_commands_are_found_in_assembly()
        {
            var emptyAssembly = new AssemblyBuilder().Build();
            var sut = new CommandScanner();
            
            var result = sut.Scan(emptyAssembly);

            Assert.Empty(result);
        }
    }
}