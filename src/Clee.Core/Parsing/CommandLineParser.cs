using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Clee.Parsing
{
    public class CommandLineParser
    {
        public static string ExtractCommandNameFrom(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }

            return Regex.Replace(input, @"^(?<cmd>\w+)(\s+(?<args>.*))?$", "${cmd}");
        }

        public static IEnumerable<Argument> ExtractArgumentsFrom(string input)
        {
            var args = ExtractArgumentsStringFrom(input);

            if (string.IsNullOrWhiteSpace(args))
            {
                return Enumerable.Empty<Argument>();
            }

            return ParseArguments(args);
        }

        public static string ExtractArgumentsStringFrom(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }

            return Regex.Replace(input, @"^(?<cmd>\w+)(\s+(?<args>.*))?$", "${args}");
        }

        public static IEnumerable<Argument> ParseArguments(string text)
        {
            var sections = SplitArgumentsIntoSections(text).ToArray();

            var index = 0;

            while (index < sections.Length)
            {
                var current = sections[index];

                if (current.StartsWith("-"))
                {
                    var name = Regex.Replace(current, "^-+(\\w)", "$1");
                    var value = "";

                    var valueIndex = index + 1;

                    if (valueIndex < sections.Length)
                    {
                        value = sections[valueIndex];

                        if (sections[valueIndex].StartsWith("-"))
                        {
                            value = "";
                        }
                        else
                        {
                            value = Regex.Replace(value, "^\"(.*)\"$", "$1");
                        }
                    }

                    yield return new Argument(name, value);
                }

                index++;
            }
        }

        public static IEnumerable<string> SplitArgumentsIntoSections(string text)
        {
            var index = 0;

            while (index < text.Length)
            {
                if (text[index] != ' ')
                {
                    var stopchar = ' ';
                    var begin = index;

                    var isQuoted = false;

                    if (text[index] == '"')
                    {
                        stopchar = '"';
                        index++;
                        isQuoted = true;
                    }

                    while (index < text.Length && text[index] != stopchar)
                    {
                        index++;
                    }

                    var end = index;

                    if (isQuoted && index < text.Length && text[index] == '"')
                    {
                        end++;
                    }

                    var count = end - begin;

                    if (count > 0)
                    {
                        yield return text.Substring(begin, count);
                    }
                }

                index++;
            }
        }

    }
}