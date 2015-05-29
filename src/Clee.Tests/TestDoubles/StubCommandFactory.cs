using System;

namespace Clee.Tests.TestDoubles
{
    public class StubCommandFactory : ICommandFactory
    {
        private readonly object _stubResult;

        public StubCommandFactory(object stubResult)
        {
            _stubResult = stubResult;
        }

        public object Resolve(Type commandType)
        {
            return _stubResult;
        }

        public void Release(object obj)
        {

        }
    }
}