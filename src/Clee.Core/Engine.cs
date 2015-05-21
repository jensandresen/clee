using System.Linq;
using System.Reflection;
using Clee.Parsing;
using Clee.Types;

namespace Clee
{
    public class Engine
    {
        private readonly CommandRegistry _registry;
        private readonly DefaultTypeFactory _typeFactory;
        private readonly ArgumentMapper _mapper;
        private readonly DefaultCommandExecutor _commandExecutor;

        public Engine()
        {
            _registry = new CommandRegistry();
            _typeFactory = new DefaultTypeFactory();
            _mapper = new ArgumentMapper();
            _commandExecutor = new DefaultCommandExecutor();

            _registry.Register(new CommandScanner().Scan(Assembly.GetExecutingAssembly()));
        }

        public void Execute(string input)
        {
            var commandName = CommandLineParser.ExtractCommandNameFrom(input);
            var commandType = _registry.Find(commandName);

            var argumentType = TypeUtils.ExtractArgumentTypesFromCommand(commandType).First();
            var argumentValues = CommandLineParser.ExtractArgumentsFrom(input).ToArray();
            var argumentInstance = _mapper.Map(argumentType, argumentValues);

            var commandInstance = _typeFactory.Resolve(commandType);

            try
            {
                _commandExecutor.Execute(commandInstance, argumentInstance);
            }
            finally
            {
                _typeFactory.Release(commandInstance);
            }
        }
    }
}