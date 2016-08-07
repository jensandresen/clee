namespace Clee.Routing
{
    public class NameAsPathStrategy : ICommandPathStrategy
    {
        public Path GeneratePathFor(CommandMetaData metaData)
        {
            return new Path(metaData.CommandName);
        }
    }
}