using System;
using System.Linq;
using System.Reflection;

namespace Clee.Tests
{
    public class SimplestConstructorSelectionStrategy : IConstructorSelectionStrategy
    {
        public ConstructorInfo GetFrom(Type type)
        {
            var temp = type
                .GetConstructors()
                .Select(x => new {Constructor = x, Arguments = x.GetParameters()})
                .OrderBy(x => x.Arguments.Length)
                .FirstOrDefault();

            return temp.Constructor;
        }
    }
}