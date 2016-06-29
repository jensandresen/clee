using System;
using System.Collections.Generic;
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

    public class StubTypeResolver : ITypeResolver
    {
        private readonly object _instance;

        public StubTypeResolver(object instance)
        {
            _instance = instance;
        }

        public T Resolve<T>()
        {
            return (T) _instance;
        }

        public void Release(object instance)
        {
            
        }
    }

    public interface ITypeResolver
    {
        T Resolve<T>();
        void Release(object instance);
    }

    public class SpyErrorHandler<T> : IErrorHandler<T> where T : Exception
    {
        private static readonly ReturnCode DefaultReturnCode = new ReturnCode(1);

        public bool wasCalled = false;

        public ReturnCode Handle(T error)
        {
            wasCalled = true;
            return DefaultReturnCode;
        }
    }

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

    public class StubErrorHandler<T> : IErrorHandler<T> where T : Exception
    {
        private readonly ReturnCode _result;

        public StubErrorHandler(ReturnCode result)
        {
            _result = result;
        }

        public ReturnCode Handle(T error)
        {
            return _result;
        }
    }

    public class ErrorHandlerEngine : IErrorHandlerEngine
    {
        private static readonly DefaultErrorHandler DefaultErrorHandler = new DefaultErrorHandler();
        private readonly Dictionary<Type, NonGenericErrorHandler> _handlers = new Dictionary<Type, NonGenericErrorHandler>();
        private readonly ITypeResolver _typeResolver;

        public ErrorHandlerEngine(ITypeResolver typeResolver)
        {
            _typeResolver = typeResolver;
        }

        public ReturnCode Handle(Exception error)
        {
            NonGenericErrorHandler customHandler;

            if (_handlers.TryGetValue(error.GetType(), out customHandler))
            {
                var result = customHandler.Handle(error);

                if (result != null)
                {
                    return result;
                }
            }

            return DefaultErrorHandler.Handle(error);
        }

        public void AddHandler<TException>(IErrorHandler<TException> errorHandler) where TException : Exception
        {
            AddHandler<TException>(error => errorHandler.Handle(error));
        }

        public void AddHandlerType<TError, THandler>()
            where TError : Exception
            where THandler : IErrorHandler<TError>
        {
            AddHandler<TError>(error =>
            {
                var handler = _typeResolver.Resolve<THandler>();

                try
                {
                    return handler.Handle(error);
                }
                finally
                {
                    _typeResolver.Release(handler);
                }
            });
        }

        public void AddHandler<TException>(Func<TException, ReturnCode> errorHandler) where TException : Exception
        {
            var nonGenericErrorHandler = new NonGenericErrorHandler(error =>
            {
                var temp = error as TException;

                if (temp == null)
                {
                    return null;
                }

                return errorHandler(temp);
            });

            AddNonGenericHandlerFor<TException>(nonGenericErrorHandler);
        }

        private void AddNonGenericHandlerFor<TException>(NonGenericErrorHandler nonGenericErrorHandler) where TException : Exception
        {
            var errorType = typeof (TException);

            if (_handlers.ContainsKey(errorType))
            {
                _handlers.Remove(errorType);
            }

            _handlers.Add(errorType, nonGenericErrorHandler);
        }

        private class NonGenericErrorHandler
        {
            private readonly Func<Exception, ReturnCode> _handlerLogic;

            public NonGenericErrorHandler(Func<Exception, ReturnCode> handlerLogic)
            {
                _handlerLogic = handlerLogic;
            }

            public ReturnCode Handle(Exception error)
            {
                return _handlerLogic(error);
            }
        }
    }

    public interface IErrorHandler<T> where T : Exception
    {
        ReturnCode Handle(T error);
    }

    public class ReturnCode
    {
        public static readonly ReturnCode Default = new ReturnCode(CommandExecutionResultsType.Error);
        
        private readonly int _errorCode;

        public ReturnCode(int errorCode)
        {
            _errorCode = errorCode;
        }

        public ReturnCode(CommandExecutionResultsType executionResult)
        {
            _errorCode = (int) executionResult;
        }

        public int ToInt()
        {
            return _errorCode;
        }

        protected bool Equals(ReturnCode other)
        {
            return _errorCode == other._errorCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((ReturnCode) obj);
        }

        public override int GetHashCode()
        {
            return _errorCode;
        }

        public static bool operator ==(ReturnCode left, ReturnCode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ReturnCode left, ReturnCode right)
        {
            return !Equals(left, right);
        }

        public static implicit operator int(ReturnCode returnCode)
        {
            return returnCode.ToInt();
        }

        public static implicit operator ReturnCode(int returnCode)
        {
            return new ReturnCode(returnCode);
        }
    }

    public class DefaultErrorHandler : IErrorHandler<Exception>
    {
        public ReturnCode Handle(Exception error)
        {
            return new ReturnCode(CommandExecutionResultsType.Error);
        }
    }
}