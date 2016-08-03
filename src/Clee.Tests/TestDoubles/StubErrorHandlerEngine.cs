using System;

namespace Clee.Tests.TestDoubles
{
    public class StubErrorHandlerEngine : IErrorHandlerEngine
    {
        private readonly CommandExecutionResultsType _result;

        public StubErrorHandlerEngine(CommandExecutionResultsType result)
        {
            _result = result;
        }

        public ReturnCode Handle(Exception error)
        {
            return new ReturnCode(_result);
        }
    }
}