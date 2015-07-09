using System;
using System.Collections.Generic;

namespace Clee
{
    public interface ICommandRegistry
    {
        Type Find(string commandName);
        CommandRegistration Register(Type commandType);
        CommandRegistration Register(string commandName, Type commandType);
        void Register(IEnumerable<Type> commandTypes);
        IEnumerable<CommandRegistration> GetAll();
        void ChangeCommandNameConvention(CommandNameConvention convention);
    }
}