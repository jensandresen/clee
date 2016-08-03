using System;

namespace Clee
{
    public interface ICreator
    {
        object CreateInstance(Type type, object[] dependencies);
    }
}