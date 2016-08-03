using System.Collections.Generic;
using System.Linq;

namespace Clee.Parsing
{
    internal class ArgumentReader
    {
        private readonly Segment[] _segments;

        public ArgumentReader(IEnumerable<Segment> segments)
        {
            _segments = segments
                .SkipWhile(x => char.IsLetter(x.Value[0]))
                .ToArray();
        }

        public IEnumerable<Argument> ReadAll()
        {
            var result = new LinkedList<Argument>();

            var index = 0;

            while (index < _segments.Length)
            {
                var argumentSegment = new ArgumentSegment(_segments[index]);
                argumentSegment.Validate();

                index++;

                ValueSegment valueSegment;
                if (TryCreateValueSegment(index, out valueSegment))
                {
                    index++;
                }

                if (argumentSegment.IsMulti)
                {
                    var list = ConvertToMultipleArguments(argumentSegment.Name, valueSegment.Value);

                    foreach (var argument in list)
                    {
                        result.AddLast(argument);
                    }
                }
                else
                {
                    result.AddLast(new Argument(
                        name: argumentSegment.Name,
                        value: valueSegment.Value
                        ));
                }
            }

            return result;
        }

        private IEnumerable<Argument> ConvertToMultipleArguments(string argumentName, string argumentValue)
        {
            for (var index = 0; index < argumentName.Length; index++)
            {
                var isLastArgument = index == argumentName.Length - 1;

                var name = argumentName[index];
                var value = "";

                if (isLastArgument)
                {
                    value = argumentValue;
                }

                yield return new Argument(
                    name: new string(name, 1),
                    value: value
                    );
            }
        }

        private bool TryCreateValueSegment(int index, out ValueSegment result)
        {
            if (index < _segments.Length)
            {
                var valueSegment = new ValueSegment(_segments[index]);

                if (valueSegment.IsValid)
                {
                    result = valueSegment;
                    return true;
                }
            }
            
            result = ValueSegment.NoValue;
            return false;
        }
    }
}