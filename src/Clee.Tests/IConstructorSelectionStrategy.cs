using System;
using System.Reflection;

namespace Clee.Tests
{
    public interface IConstructorSelectionStrategy
    {
        ConstructorInfo GetFrom(Type type);
    }
}