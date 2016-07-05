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
            var objectGraph = new ObjectGraph(_constructorSelectionStrategy, _creator);
            var relationship = objectGraph.CreateFor(type);

            _relationships.Add(relationship.Instance, relationship);

            return relationship.Instance;
        }

        private class ObjectGraph
        {
            private readonly IConstructorSelectionStrategy _constructorSelectionStrategy;
            private readonly ICreator _creator;

            public ObjectGraph(IConstructorSelectionStrategy constructorSelectionStrategy, ICreator creator)
            {
                _constructorSelectionStrategy = constructorSelectionStrategy;
                _creator = creator;
            }

            public Relationship CreateFor(Type type)
            {
                return ResolveRelationshipFor(type, new Type[0]);
            }

            private Relationship ResolveRelationshipFor(Type type, IEnumerable<Type> parentDependencyGraph)
            {
                var constructor = _constructorSelectionStrategy.GetFrom(type);
                var relationships = new LinkedList<Relationship>();

                var newDependencyGraph = Enumerable
                    .Concat(parentDependencyGraph, new[] {type})
                    .ToArray();

                var graph = new HashSet<Type>(newDependencyGraph);
                
                foreach (var parameter in constructor.GetParameters())
                {
                    if (graph.Contains(parameter.ParameterType))
                    {
                        throw new CircularDependencyException();
                    }   

                    var rel = ResolveRelationshipFor(
                        type: parameter.ParameterType,
                        parentDependencyGraph: newDependencyGraph
                        );

                    relationships.AddLast(rel);
                }

                return new Relationship(
                    instanceFactory: () =>
                    {
                        var dependencies = relationships
                            .Select(x => x.Instance)
                            .ToArray();

                        return CreateInstance(type, dependencies);
                    },
                    instanceType: type,
                    dependencies: relationships
                    );
            }

            private object CreateInstance(Type type, object[] dependencies)
            {
                return _creator.CreateInstance(type, dependencies);
            }

            public IEnumerable<Type> ConvertToTypeList(Relationship relationship)
            {
                var list = new LinkedList<Type>();
                list.AddLast(relationship.InstanceType);

                foreach (var dependency in relationship.Dependencies)
                {
                    var d = ConvertToTypeList(dependency);

                    foreach (var dt in d)
                    {
                        list.AddLast(dt);
                    }
                }

                return list;
            }
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
            private readonly Func<object> _instanceFactory;
            private object _instance;

            public Relationship(Func<object> instanceFactory, Type instanceType, IEnumerable<Relationship> dependencies)
            {
                _instanceFactory = instanceFactory;
                InstanceType = instanceType;
                Dependencies = dependencies;
            }

            public object Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = _instanceFactory();
                    }

                    return _instance;
                }
            }

            public Type InstanceType { get; private set; }
            public IEnumerable<Relationship> Dependencies { get; private set; }
        }
    }
}