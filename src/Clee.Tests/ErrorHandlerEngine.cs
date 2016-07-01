using System;
using System.Collections.Generic;

namespace Clee.Tests
{
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
}