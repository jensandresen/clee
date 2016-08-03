using System;

namespace Clee
{
    public interface IErrorHandlerEngine
    {
        ReturnCode Handle(Exception error);
    }
}