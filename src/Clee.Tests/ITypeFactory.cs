using System;

namespace Clee.Tests
{
    public interface ITypeFactory
    {
        object Resolve(Type commandType);
        void Release(object obj);
    }
}