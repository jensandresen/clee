using System;
using System.Collections.Generic;

namespace Clee
{
    public interface ICommandRegistry
    {
        Type Find(string commandName);
        void Register(Type commandType);
        void Register(IEnumerable<Type> commandTypes);
    }
}