namespace Clee.Tests.TestDoubles.Dummies
{
    public class MultiArgumentCommand : ICommand<FooArgument>, ICommand<BarArgument>
    {
        public void Execute(FooArgument args) { }
        public void Execute(BarArgument args) { }
    }
}