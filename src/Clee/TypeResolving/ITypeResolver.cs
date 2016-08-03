namespace Clee.TypeResolving
{
    public interface ITypeResolver
    {
        T Resolve<T>();
        void Release(object instance);
    }
}