using System;
using System.Collections.Generic;
using System.Linq;

namespace Clee.Tests
{
    public class TypeContainer
    {
        private readonly SimplestConstructorSelectionStrategy _constructorSelectionStrategy = new SimplestConstructorSelectionStrategy();
        private readonly Dictionary<object, Relationship> _relationships = new Dictionary<object, Relationship>();
        private readonly ICreator _creator;

        public TypeContainer(ICreator creator)
        {
            _creator = creator;
        }

        public T Resolve<T>()
        {
            var instance = Resolve(typeof (T));
            return (T) instance;
        }

        public object Resolve(Type type)
        {
            var relationship = ResolveRelationshipFor(type);
            _relationships.Add(relationship.Instance, relationship);

            return relationship.Instance;
        }

        private Relationship ResolveRelationshipFor(Type type)
        {
            var constructor = _constructorSelectionStrategy.GetFrom(type);
            var relationships = new LinkedList<Relationship>();

            foreach (var parameter in constructor.GetParameters())
            {
                var rel = ResolveRelationshipFor(parameter.ParameterType);
                relationships.AddLast(rel);
            }

            var dependencies = relationships
                .Select(x => x.Instance)
                .ToArray();

            var instance = CreateInstance(type, dependencies);

            return new Relationship(
                instance: instance,
                instanceType: type,
                dependencies: relationships
                );
        }

        private object CreateInstance(Type type, object[] dependencies)
        {
            return _creator.CreateInstance(type, dependencies);
        }

        public void Release(object instance)
        {
            Relationship relationship;

            if (_relationships.TryGetValue(instance, out relationship))
            {
                ReleaseRelationship(relationship);
                _relationships.Remove(instance);
            }
            else
            {
                var disposable = instance as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private void ReleaseRelationship(Relationship relationship)
        {
            foreach (var rel in relationship.Dependencies)
            {
                ReleaseRelationship(rel);
            }

            var disposable = relationship.Instance as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        private class Relationship
        {
            public Relationship(object instance, Type instanceType, IEnumerable<Relationship> dependencies)
            {
                Instance = instance;
                InstanceType = instanceType;
                Dependencies = dependencies;
            }

            public object Instance { get; private set; }
            public Type InstanceType { get; private set; }
            public IEnumerable<Relationship> Dependencies { get; private set; }
        }
    }
}