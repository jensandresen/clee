namespace Clee.Tests.TestDoubles.Dummies
{
    public class SingleArgumentCommand : ICommand<FooArgument>
    {
        public void Execute(FooArgument args) { }
    }
}