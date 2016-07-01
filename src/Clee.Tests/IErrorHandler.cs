using System;

namespace Clee.Tests
{
    public interface IErrorHandler<T> where T : Exception
    {
        ReturnCode Handle(T error);
    }
}