using System.Collections.Generic;

namespace Clee.Mapping
{
    public interface IArgumentMapper
    {
        void Map(Command command, IEnumerable<Argument> arguments);
    }
}