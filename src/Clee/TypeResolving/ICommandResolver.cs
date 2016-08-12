using System;

namespace Clee.TypeResolving
{
    public interface ICommandResolver
    {
        T Resolve<T>() where T : Command;
        Command Resolve(Type commandType);

        void Release(Command command);
    }
}