using Clee.TypeResolving;

namespace Clee.Tests.TestDoubles
{
    public class StubCommandResolver : ICommandResolver
    {
        private readonly Command _result;

        public StubCommandResolver(Command result)
        {
            _result = result;
        }

        public T Resolve<T>() where T : Command
        {
            return (T) _result;
        }

        public void Release(Command command)
        {
            
        }
    }
}