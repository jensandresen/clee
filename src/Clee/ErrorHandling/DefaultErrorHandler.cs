using System;

namespace Clee.ErrorHandling
{
    public class DefaultErrorHandler : IErrorHandler<Exception>
    {
        public ReturnCode Handle(Exception error)
        {
            return new ReturnCode(CommandExecutionResultsType.Error);
        }
    }
}