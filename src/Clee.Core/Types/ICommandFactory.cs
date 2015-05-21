using System;

namespace Clee.Types
{
    public interface ICommandFactory
    {
        object Resolve(Type commandType);
        void Release(object obj);
    }
}