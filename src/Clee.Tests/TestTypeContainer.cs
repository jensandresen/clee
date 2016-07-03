using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using Xunit;

namespace Clee.Tests
{
    public class TestTypeContainer
    {
        [Fact]
        public void can_resolve_simple_type_by_generic_argument()
        {
            var sut = new TypeContainerBuilder().Build();
            var result = sut.Resolve<NoArgumentConstructorType>();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<NoArgumentConstructorType>(result);
        }

        [Fact]
        public void can_resolve_simple_type_by_type_argument()
        {
            var sut = new TypeContainerBuilder().Build();
            var result = sut.Resolve(typeof(NoArgumentConstructorType));

            Assert.NotNull(result);
            Assert.IsAssignableFrom<NoArgumentConstructorType>(result);
        }

        [Fact]
        public void can_resolve_semi_complex_type_with_constructor_arguments()
        {
            var sut = new TypeContainerBuilder().Build();
            var result = sut.Resolve<SingleArgumentConstructorType>();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<SingleArgumentConstructorType>(result);
        }

        [Fact]
        public void can_resolve_complex_type_with_multiple_constructor_arguments()
        {
            var sut = new TypeContainerBuilder().Build();
            var result = sut.Resolve<MultipleArgumentConstructorType>();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<MultipleArgumentConstructorType>(result);
        }

        [Fact]
        public void can_resolve_complex_type_with_multiple_nested_constructor_arguments()
        {
            var sut = new TypeContainerBuilder().Build();
            var result = sut.Resolve<NestedMultipleArgumentConstructorType>();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<NestedMultipleArgumentConstructorType>(result);
        }

        [Fact]
        public void disposable_instances_are_disposed_when_released()
        {
            var mock = new Mock<IDisposable>();
            var sut = new TypeContainerBuilder().Build();
            
            sut.Release(mock.Object);

            mock.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void disposable_dependencies_are_disposed_when_root_is_released()
        {
            var dep1 = new DisposableType();
            var dep2 = new DisposableType();
            var owner = new DisposableOwner(dep1, dep2);

            var list = new Queue();
            list.Enqueue(dep1);
            list.Enqueue(dep2);
            list.Enqueue(owner);

            var creator = new HumbleCreator((type, dependencies) =>
            {
                return list.Dequeue();
            });

            var sut = new TypeContainerBuilder()
                .WithCreator(creator)
                .Build();

            var result = sut.Resolve<DisposableOwner>();

            sut.Release(result);

            Assert.True(owner.wasDisposed);
            Assert.True(dep1.wasDisposed);
            Assert.True(dep2.wasDisposed);
        }

        #region dummy types

        private class DisposableType : IDisposable
        {
            public bool wasDisposed = false;
            
            public void Dispose()
            {
                wasDisposed = true;
            }
        }

        private class DisposableOwner : DisposableType
        {
            public DisposableOwner(DisposableType arg1, DisposableType arg2)
            {
                Arg1 = arg1;
                Arg2 = arg2;
            }

            public DisposableType Arg1 { get; private set; }
            public DisposableType Arg2 { get; private set; }
        }

        private class NoArgumentConstructorType
        {
            
        }

        private class SingleArgumentConstructorType
        {
            public SingleArgumentConstructorType(NoArgumentConstructorType arg1)
            {
                
            }
        }

        private class MultipleArgumentConstructorType
        {
            public MultipleArgumentConstructorType(NoArgumentConstructorType arg1, NoArgumentConstructorType arg2)
            {
                
            }
        }

        private class NestedMultipleArgumentConstructorType
        {
            public NestedMultipleArgumentConstructorType(NoArgumentConstructorType arg1, SingleArgumentConstructorType arg2, MultipleArgumentConstructorType arg3)
            {
                
            }
        }

        #endregion
    }

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

    public interface ICreator
    {
        object CreateInstance(Type type, object[] dependencies);
    }

    public class DefaultCreator : ICreator
    {
        public object CreateInstance(Type type, object[] dependencies)
        {
            return Activator.CreateInstance(type, dependencies);
        }
    }

    public interface IConstructorSelectionStrategy
    {
        ConstructorInfo GetFrom(Type type);
    }

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