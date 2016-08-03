using System;

namespace Clee.TypeResolving
{
    public class DefaultCreator : ICreator
    {
        public object CreateInstance(Type type, object[] dependencies)
        {
            return Activator.CreateInstance(type, dependencies);
        }
    }
}