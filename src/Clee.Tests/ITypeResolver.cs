namespace Clee.Tests
{
    public interface ITypeResolver
    {
        T Resolve<T>();
        void Release(object instance);
    }
}