namespace Clee
{
    public interface ICommandExecutor
    {
        void Execute(object command, object arguments);
    }
}