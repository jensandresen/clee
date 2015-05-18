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

        [Fact]
        public void returns_expected_when_commands_are_found_in_an_assembly()
        {
            var sut = new CommandScanner();
            var result = sut.Scan(this.GetType().Assembly);

            Assert.NotEmpty(result);
        }

        [Fact]
        public void returns_expected_when_namespace_filter_is_applied()
        {
            var sut = new CommandScanner();
            var result = sut.Scan(this.GetType().Assembly, this.GetType().Namespace);

            Assert.Equal(new[]{typeof(FooCommand)}, result);
        }


        public class FooData : ICommandArguments
        {
             
        }

        public class FooCommand : ICommand<FooData>
        {
             
        }
    }
}