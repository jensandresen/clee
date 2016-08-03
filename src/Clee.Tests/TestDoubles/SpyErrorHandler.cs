using System;
using Clee.ErrorHandling;

namespace Clee.Tests.TestDoubles
{
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
}