using Clee.TypeResolving;

namespace Clee.Tests.Builders
{
    internal class TypeContainerBuilder
    {
        private ICreator _creator;

        public TypeContainerBuilder()
        {
            _creator = new DefaultCreator();
        }

        public TypeContainerBuilder WithCreator(ICreator creator)
        {
            _creator = creator;
            return this;
        }

        public TypeContainer Build()
        {
            return new TypeContainer(_creator);
        }
    }
}