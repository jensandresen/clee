using System;

namespace Clee.Tests
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