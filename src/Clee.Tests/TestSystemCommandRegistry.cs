using System.Linq;
using Clee.SystemCommands;
using Xunit;

namespace Clee.Tests
{
    public class TestSystemCommandRegistry
    {
        [Fact]
        public void returns_expected_system_commands()
        {
            var sut = SystemCommandRegistry.CreateAndInitialize();
            
            var result = sut
                .GetAll()
                .Select(x => x.CommandName)
                .ToArray();

            var expected = new[]
            {
                "help", 
                "list"
            };

            Assert.Equal(expected, result);
        }
 
    }
}