namespace Clee
{
    public interface ICommandPathStrategy
    {
        Path GeneratePathFor(CommandMetaData metaData);
    }
}