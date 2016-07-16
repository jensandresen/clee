namespace Clee.Tests
{
    public interface ICommandPathStrategy
    {
        Path GeneratePathFor(CommandMetaData metaData);
    }
}