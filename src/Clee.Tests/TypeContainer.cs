using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clee.Tests
{
    public class TypeContainer
    {
        private readonly SimplestConstructorSelectionStrategy _constructorSelectionStrategy = new SimplestConstructorSelectionStrategy();
        private readonly Dictionary<object, Relationship> _relationships = new Dictionary<object, Relationship>();
        
        private readonly Dictionary<Type, Type> _typeMap = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, Func<object>> _typeFactories = new Dictionary<Type, Func<object>>();
        
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
            var relationship = ResolveRelationshipFor(type, new Type[0]);
            _relationships.Add(relationship.Instance, relationship);

            return relationship.Instance;
        }

        private Type GetConcreteTypeFor(Type aType)
        {
            if (aType.IsAbstract)
            {
                Type concreteTypeFactory;

                if (!_typeMap.TryGetValue(aType, out concreteTypeFactory))
                {
                    throw new UnresolveableDependencyException();
                }

                return concreteTypeFactory;
            }

            return aType;
        }

        private Relationship ResolveRelationshipFor(Type type, IEnumerable<Type> parentDependencyGraph)
        {
            var relationships = new LinkedList<Relationship>();

            parentDependencyGraph = parentDependencyGraph
                .Concat(new[] { type })
                .ToArray();

            var existingDependencyTypes = new HashSet<Type>(parentDependencyGraph);
            
            var concreteType = GetConcreteTypeFor(type);
            var constructorParameters = GetConstructorParametersFor(concreteType);

            foreach (var parameter in constructorParameters)
            {
                if (existingDependencyTypes.Contains(parameter.ParameterType))
                {
                    throw new CircularDependencyException();
                }

                Func<object> typeFactory;

                if (_typeFactories.TryGetValue(parameter.ParameterType, out typeFactory))
                {
                    relationships.AddLast(new Relationship(
                        instanceFactory: typeFactory,
                        instanceType: type,
                        dependencies: Enumerable.Empty<Relationship>()
                        ));
                }
                else
                {
                    var relationship = ResolveRelationshipFor(
                        type: parameter.ParameterType,
                        parentDependencyGraph: parentDependencyGraph
                        );

                    relationships.AddLast(relationship);
                }
            }

            return new Relationship(
                instanceFactory: () =>
                {
                    var dependencies = relationships
                        .Select(x => x.Instance)
                        .ToArray();

                    return _creator.CreateInstance(concreteType, dependencies);
                },
                instanceType: concreteType,
                dependencies: relationships
                );
        }

        private IEnumerable<ParameterInfo> GetConstructorParametersFor(Type concreteType)
        {
            var constructor = _constructorSelectionStrategy.GetFrom(concreteType);
            return constructor.GetParameters();
        }

        public void Register<TAbstraction, TImplementation>() where TImplementation : TAbstraction
        {
            Register(typeof(TAbstraction), typeof(TImplementation));
        }

        public void Register(Type abstraction, Type implementation)
        {
            if (!abstraction.IsAssignableFrom(implementation))
            {
                throw new NotSupportedTypeRegistrationException();
            }

            _typeMap.Add(abstraction, implementation);
        }

        public void Register<TAbstraction>(Func<TAbstraction> typeFactory)
        {
            _typeFactories.Add(typeof(TAbstraction), () => typeFactory());
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

    public class NotSupportedTypeRegistrationException : Exception
    {

    }
}