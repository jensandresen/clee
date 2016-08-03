using System;
using System.Reflection;

namespace Clee
{
    public interface IConstructorSelectionStrategy
    {
        ConstructorInfo GetFrom(Type type);
    }
}