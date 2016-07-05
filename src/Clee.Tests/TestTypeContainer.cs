using System;
using System.Collections;
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

        [Fact]
        public void throws_exception_on_circular_references_in_object_graph()
        {
            var sut = new TypeContainerBuilder().Build();
            Assert.Throws<CircularDependencyException>(() => sut.Resolve<CircularRoot>());
        }

        [Fact]
        public void supports_abstract_dependencies()
        {
            var sut = new TypeContainerBuilder().Build();
            sut.Register<IAbstractDependency, AbstractDependencyImplementation>();
            
            var result = sut.Resolve<ConcreteRoot>();

            Assert.NotNull(result);
            Assert.NotNull(result.Dependency);
            Assert.IsType<AbstractDependencyImplementation>(result.Dependency);
        }

        [Fact]
        public void throws_exception_if_no_concrete_type_has_been_registered_for_abstract_dependency()
        {
            var sut = new TypeContainerBuilder().Build();
            Assert.Throws<UnresolveableDependencyException>(() => sut.Resolve<ConcreteRoot>());
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

        private class CircularRoot
        {
            public CircularRoot(CircularDependencyLevel1 dep1)
            {

            }
        }

        private class CircularDependencyLevel1
        {
            public CircularDependencyLevel1(CircularDependencyLevel2 dep1)
            {

            }
        }

        private class CircularDependencyLevel2
        {
            public CircularDependencyLevel2(CircularDependencyLevel3 dep)
            {

            }
        }

        private class CircularDependencyLevel3
        {
            public CircularDependencyLevel3(CircularRoot root)
            {

            }
        }

        public class ConcreteRoot
        {
            private readonly IAbstractDependency _abstractDependency;

            public ConcreteRoot(IAbstractDependency abstractDependency)
            {
                _abstractDependency = abstractDependency;
            }

            public IAbstractDependency Dependency
            {
                get { return _abstractDependency; }
            }
        }

        public interface IAbstractDependency
        {
             
        }

        public class AbstractDependencyImplementation : IAbstractDependency
        {
             
        }

        #endregion
    }

    public class UnresolveableDependencyException : Exception
    {
    }

    public class CircularDependencyException : Exception
    {

    }
}