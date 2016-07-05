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
            var objectGraph = new ObjectGraph(_constructorSelectionStrategy, _creator, aType =>
            {
                Type concreteType;
                
                if (_typeMap.TryGetValue(aType, out concreteType))
                {
                    return concreteType;
                }

                return aType;
            });

            var relationship = objectGraph.CreateFor(type);

            _relationships.Add(relationship.Instance, relationship);

            return relationship.Instance;
        }

        private readonly Dictionary<Type, Type> _typeMap = new Dictionary<Type, Type>();

        public void Register<TAbstraction, TImplementation>()
        {
            _typeMap.Add(typeof(TAbstraction), typeof(TImplementation));
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

        private class ObjectGraph
        {
            private readonly IConstructorSelectionStrategy _constructorSelectionStrategy;
            private readonly ICreator _creator;
            private readonly Func<Type, Type> _typeTranslator;

            public ObjectGraph(IConstructorSelectionStrategy constructorSelectionStrategy, ICreator creator, Func<Type, Type> typeTranslator)
            {
                _constructorSelectionStrategy = constructorSelectionStrategy;
                _creator = creator;
                _typeTranslator = typeTranslator;
            }

            public Relationship CreateFor(Type type)
            {
                return ResolveRelationshipFor(type, new Type[0]);
            }

            private Relationship ResolveRelationshipFor(Type type, IEnumerable<Type> parentDependencyGraph)
            {
                type = GetConcreteTypeFor(type);
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

            private Type GetConcreteTypeFor(Type type)
            {
                return _typeTranslator(type);
            }

            private object CreateInstance(Type type, object[] dependencies)
            {
                return _creator.CreateInstance(type, dependencies);
            }
        }
    }
}