using System;

namespace Clee
{
    public interface ICommandFactory
    {
        object Resolve(Type commandType);
        void Release(object obj);
    }
}