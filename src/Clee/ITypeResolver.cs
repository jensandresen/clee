namespace Clee
{
    public interface ITypeResolver
    {
        T Resolve<T>();
        void Release(object instance);
    }
}