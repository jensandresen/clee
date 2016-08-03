using System;
using System.Reflection;

namespace Clee.TypeResolving
{
    public interface IConstructorSelectionStrategy
    {
        ConstructorInfo GetFrom(Type type);
    }
}