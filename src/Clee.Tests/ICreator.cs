using System;

namespace Clee.Tests
{
    public interface ICreator
    {
        object CreateInstance(Type type, object[] dependencies);
    }
}