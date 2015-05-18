using System;

namespace Clee.Types
{
    public interface ITypeFactory
    {
        object Resolve(Type commandType);
        void Release(object obj);
    }
}