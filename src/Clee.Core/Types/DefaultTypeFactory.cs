using System;

namespace Clee.Types
{
    public class DefaultTypeFactory : ITypeFactory
    {
        public object Resolve(Type commandType)
        {
            return Activator.CreateInstance(commandType);
        }

        public void Release(object obj)
        {
            var disposable = obj as IDisposable;
            
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}