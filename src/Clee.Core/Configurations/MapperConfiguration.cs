using System;
using System.Collections.Generic;
using Clee.Parsing;

namespace Clee.Configurations
{
    internal class MapperConfiguration : IMapperConfiguration
    {
        private readonly List<Action<IArgumentMapper>> _modifiers = new List<Action<IArgumentMapper>>();
        private IArgumentMapper _mapper;

        public MapperConfiguration()
        {
            _mapper = new DefaultArgumentMapper();
        }

        public IMapperConfiguration Use(IArgumentMapper mapper)
        {
            _mapper = mapper;
            return this;
        }

        public IMapperConfiguration AddParser<T>(IValueParser parser)
        {
            _modifiers.Add(m => m.AddParser<T>(parser));
            return this;
        }

        public IMapperConfiguration AddParser(Type targetType, IValueParser parser)
        {
            _modifiers.Add(m => m.AddParser(targetType, parser));
            return this;
        }

        public IArgumentMapper Build()
        {
            foreach (var modify in _modifiers)
            {
                modify(_mapper);
            }

            return _mapper;
        }
    }
}