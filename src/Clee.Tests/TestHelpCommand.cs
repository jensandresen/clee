using Clee.Tests.TestDoubles;
using Xunit;

namespace Clee.Tests
{
    public class TestHelpCommand
    {
        [Fact]
        public void writes_expected_to_output_when_executed()
        {
            var spyOutputWriter = new SpyOutputWriter();

            var sut = CleeEngine.CreateDefault();
            sut.SetOutputWriter(spyOutputWriter);

            sut.Execute("help");

            Assert.StartsWith("Usage", spyOutputWriter.Output.ToString());
        }
    }
}