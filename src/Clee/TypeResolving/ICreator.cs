using System;

namespace Clee.TypeResolving
{
    public interface ICreator
    {
        object CreateInstance(Type type, object[] dependencies);
    }
}