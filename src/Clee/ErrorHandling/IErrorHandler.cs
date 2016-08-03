using System;

namespace Clee.ErrorHandling
{
    public interface IErrorHandler<T> where T : Exception
    {
        ReturnCode Handle(T error);
    }
}