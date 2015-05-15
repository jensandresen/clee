namespace Clee.Tests
{
    public interface ICommand<T> where T : ICommandArguments, new()
    {

    }
}