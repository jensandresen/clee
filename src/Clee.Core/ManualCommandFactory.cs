using System;
using System.Collections.Generic;

namespace Clee
{
    public class ManualCommandFactory : ICommandFactory
    {
        private readonly Dictionary<Type, Func<object>> _creators = new Dictionary<Type, Func<object>>();

        public void Add<T>(Func<T> creator)
        {
            _creators.Add(typeof(T), creator as Func<object>);
        }

        public object Resolve(Type commandType)
        {
            Func<object> creator;
            
            if (_creators.TryGetValue(commandType, out creator))
            {
                return creator();
            }

            throw new NotSupportedException(string.Format("Unable to resolve type {0}", commandType.FullName));
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