using System;

namespace Clee.Tests
{
    public interface IErrorHandlerEngine
    {
        ReturnCode Handle(Exception error);
    }
}