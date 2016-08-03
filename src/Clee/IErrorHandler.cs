using System;

namespace Clee
{
    public interface IErrorHandler<T> where T : Exception
    {
        ReturnCode Handle(T error);
    }
}