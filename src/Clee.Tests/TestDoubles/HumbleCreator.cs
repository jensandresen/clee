using System;
using Clee.TypeResolving;

namespace Clee.Tests.TestDoubles
{
    public class HumbleCreator : ICreator
    {
        private readonly Func<Type, object[], object> _creatorLogic;

        public HumbleCreator(Func<Type, object[], object> creatorLogic)
        {
            _creatorLogic = creatorLogic;
        }

        public object CreateInstance(Type type, object[] dependencies)
        {
            return _creatorLogic(type, dependencies);
        }
    }
}