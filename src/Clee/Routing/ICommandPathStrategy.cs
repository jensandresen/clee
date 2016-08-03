namespace Clee.Routing
{
    public interface ICommandPathStrategy
    {
        Path GeneratePathFor(CommandMetaData metaData);
    }
}