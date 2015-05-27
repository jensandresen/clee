using System;
using Clee.Parsing;

namespace Clee.Configurations
{
    public interface IMapperConfiguration
    {
        IMapperConfiguration Use(IArgumentMapper mapper);
        IMapperConfiguration AddParser<T>(IValueParser parser);
        IMapperConfiguration AddParser(Type targetType, IValueParser parser);
    }
}