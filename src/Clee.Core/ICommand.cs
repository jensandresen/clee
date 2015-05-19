namespace Clee
{
    public interface ICommand<T> where T : ICommandArguments, new()
    {
        void Execute(T args);
    }
}