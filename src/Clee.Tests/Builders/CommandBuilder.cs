using Moq;

namespace Clee.Tests.Builders
{
    internal class CommandBuilder
    {
        public Command Build()
        {
            var dummy = new Mock<Command>().Object;
            return dummy;
        }
    }
}