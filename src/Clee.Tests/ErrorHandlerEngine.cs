using System;
using System.Collections.Generic;
using Xunit;

namespace Clee.Tests
{
    public class TestErrorHandlerEngine
    {
        [Fact]
        public void returns_expected_error_result_if_no_handlers_has_been_added()
        {
            var sut = new ErrorHandlerEngine();
            var result = sut.Handle(new Exception());

            Assert.Equal(new ReturnCode(CommandExecutionResultsType.Error), result);
        }

        [Fact]
        public void returns_expected_error_code_from_custom_error_handler()
        {
            var expectedResult = 1;

            var sut = new ErrorHandlerEngine();
            sut.AddHandler<ArgumentNullException>((e) =>
            {
                return new ReturnCode(expectedResult);
            });
            
            var result = sut.Handle(new ArgumentNullException());

            Assert.Equal(expectedResult, result.ToInt());
        }
    }

    public class ErrorHandlerEngine : IErrorHandlerEngine
    {
        private readonly Dictionary<Type, Func<Exception, ReturnCode>> _handlers = new Dictionary<Type, Func<Exception, ReturnCode>>(); 

        public ReturnCode Handle(Exception error)
        {
            Func<Exception, ReturnCode> customHandler;

            if (_handlers.TryGetValue(error.GetType(), out customHandler))
            {
                var result = customHandler(error);

                if (result != null)
                {
                    return result;
                }
            }

            var defaultErrorHandler = new DefaultErrorHandler();
            return defaultErrorHandler.Handle(error);
        }

        public void AddHandler<TException>(Func<TException, ReturnCode> errorHandler) where TException : Exception
        {
            Func<Exception, ReturnCode> downgradedHandler = (error) =>
            {
                var temp = error as TException;
                if (temp == null)
                {
                    return null;
                }

                return errorHandler(temp);
            };

            var errorType = typeof(TException);

            if (_handlers.ContainsKey(errorType))
            {
                _handlers.Remove(errorType);
            }

            _handlers.Add(errorType, downgradedHandler);
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