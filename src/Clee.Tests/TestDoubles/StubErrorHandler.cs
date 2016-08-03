using System;
using Clee.ErrorHandling;

namespace Clee.Tests.TestDoubles
{
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
}