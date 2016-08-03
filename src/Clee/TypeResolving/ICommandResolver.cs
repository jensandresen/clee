namespace Clee.TypeResolving
{
    public interface ICommandResolver
    {
        T Resolve<T>() where T : Command;
        void Release(Command command);
    }
}