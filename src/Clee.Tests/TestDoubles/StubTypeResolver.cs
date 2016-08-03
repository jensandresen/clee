using Clee.TypeResolving;

namespace Clee.Tests.TestDoubles
{
    public class StubTypeResolver : ITypeResolver
    {
        private readonly object _instance;

        public StubTypeResolver(object instance)
        {
            _instance = instance;
        }

        public T Resolve<T>()
        {
            return (T) _instance;
        }

        public void Release(object instance)
        {
            
        }
    }
}