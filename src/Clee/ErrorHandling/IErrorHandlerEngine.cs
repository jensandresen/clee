using System;

namespace Clee.ErrorHandling
{
    public interface IErrorHandlerEngine
    {
        ReturnCode Handle(Exception error);
    }
}