using System;

namespace Clee
{
    public abstract class CommandNameConvention
    {
        public virtual string ExtractCommandNameFrom(Type commandType)
        {
            return ExtractCommandNameFrom(commandType.Name);
        }

        public abstract string ExtractCommandNameFrom(string typeName);
    }
}