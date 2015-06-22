namespace Clee.SystemCommands
{
    public class SystemCommandRegistry : DefaultCommandRegistry
    {
        public void Initialize()
        {
            var currentType = GetType();

            var scanner = new CommandScanner();
            var systemCommandTypes = scanner.Scan(currentType.Assembly, currentType.Namespace);

            Register(systemCommandTypes);
        }

        public static SystemCommandRegistry CreateAndInitialize()
        {
            var registry = new SystemCommandRegistry();
            registry.Initialize();

            return registry;
        }
    }
}