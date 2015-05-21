using System;
using Clee.Parsing;

namespace Clee
{
    public interface IArgumentMapper
    {
        void AddParser<T>(IValueParser parser);
        void AddParser(Type targetType, IValueParser parser);
        object Map(Type argumentType, Argument[] argumentValues);
    }
}