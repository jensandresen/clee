namespace Clee.Tests.Builders
{
    internal class CleeEngineBuilder
    {
        public CleeEngine Build()
        {
            return CleeEngine.Create(cfg => { });
        }
    }
}