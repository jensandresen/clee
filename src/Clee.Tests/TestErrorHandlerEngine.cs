using System;
using Clee.Tests.Builders;
using Clee.Tests.TestDoubles;
using Clee.TypeResolving;
using Moq;
using Xunit;

namespace Clee.Tests
{
    public class TestErrorHandlerEngine
    {
        [Fact]
        public void returns_expected_error_result_if_no_handlers_has_been_added()
        {
            var sut = new ErrorHandlerEngineBuilder().Build();
            var result = sut.Handle(new Exception());

            Assert.Equal(new ReturnCode(CommandExecutionResultsType.Error), result);
        }

        [Fact]
        public void returns_expected_error_code_from_custom_inline_error_handler()
        {
            var expected = new ReturnCode(1);
            
            var sut = new ErrorHandlerEngineBuilder().Build();
            sut.AddHandler<ArgumentNullException>((error) =>
            {
                return expected;
            });
            
            var result = sut.Handle(new ArgumentNullException());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void returns_expected_error_code_from_custom_error_handler()
        {
            var expected = new ReturnCode(1);

            var sut = new ErrorHandlerEngineBuilder().Build();
            sut.AddHandler(new StubErrorHandler<ArgumentNullException>(expected));
            
            var result = sut.Handle(new ArgumentNullException());

            Assert.Equal(expected, result);
        }

        [Fact]
        public void can_resolve_error_handler_by_type_generic_definition()
        {
            var dummyErrorHandler = new SpyErrorHandler<ArgumentNullException>();

            var mock = new Mock<ITypeResolver>();
            mock
                .Setup(x => x.Resolve<SpyErrorHandler<ArgumentNullException>>())
                .Returns(dummyErrorHandler);

            var sut = new ErrorHandlerEngineBuilder()
                .WithTypeResolver(mock.Object)    
                .Build();

            sut.AddHandlerType<ArgumentNullException, SpyErrorHandler<ArgumentNullException>>();

            sut.Handle(new ArgumentNullException());

            mock.Verify(x => x.Resolve<SpyErrorHandler<ArgumentNullException>>());
        }

        [Fact]
        public void invokes_the_resolved_error_handler()
        {
            var spyErrorHandler = new SpyErrorHandler<ArgumentNullException>();

            var sut = new ErrorHandlerEngineBuilder()
                .WithTypeResolver(new StubTypeResolver(spyErrorHandler))
                .Build();

            sut.AddHandlerType<ArgumentNullException, SpyErrorHandler<ArgumentNullException>>();

            sut.Handle(new ArgumentNullException());

            Assert.True(spyErrorHandler.wasCalled);
        }

        [Fact]
        public void releases_the_resolved_error_handler()
        {
            var dummyErrorHandler = new SpyErrorHandler<ArgumentNullException>();

            var mock = new Mock<ITypeResolver>();
            mock
                .Setup(x => x.Resolve<SpyErrorHandler<ArgumentNullException>>())
                .Returns(dummyErrorHandler);

            var sut = new ErrorHandlerEngineBuilder()
                .WithTypeResolver(mock.Object)
                .Build();

            sut.AddHandlerType<ArgumentNullException, SpyErrorHandler<ArgumentNullException>>();

            sut.Handle(new ArgumentNullException());

            mock.Verify(x => x.Release(dummyErrorHandler));
        }
    }
}