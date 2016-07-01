using Moq;

namespace Clee.Tests
{
    internal class ErrorHandlerEngineBuilder
    {
        private ITypeResolver _typeResolver;

        public ErrorHandlerEngineBuilder()
        {
            _typeResolver = new Mock<ITypeResolver>().Object;
        }

        public ErrorHandlerEngineBuilder WithTypeResolver(ITypeResolver typeResolver)
        {
            _typeResolver = typeResolver;
            return this;
        }

        public ErrorHandlerEngine Build()
        {
            return new ErrorHandlerEngine(_typeResolver);
        }
    }
}