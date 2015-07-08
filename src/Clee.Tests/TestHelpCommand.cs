using System.Linq;
using Clee.SystemCommands;
using Clee.Tests.Builders;
using Clee.Tests.TestDoubles;
using Xunit;

namespace Clee.Tests
{
    public class TestHelpCommand
    {
        [Fact]
        public void executes_expected_help_command()
        {
            var sut = new CleeEngineBuilder().Build();
            sut.Execute("help");

            Assert.Equal("help", sut.History.Single().CommandName);
            Assert.Equal(typeof(HelpCommand), sut.History.Single().ImplementationType);
        }

        [Fact]
        public void writes_expected_to_output_when_executed()
        {
            var spyOutputWriter = new SpyOutputWriter();

            var sut = new CleeEngineBuilder().Build();
            sut.SetOutputWriter(spyOutputWriter);

            sut.Execute("help");

            Assert.StartsWith("Usage", spyOutputWriter.Output.ToString());
        }
    }
}