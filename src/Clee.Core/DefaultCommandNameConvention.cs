using System.Text.RegularExpressions;

namespace Clee
{
    public class DefaultCommandNameConvention : CommandNameConvention
    {
        public override string ExtractCommandNameFrom(string typeName)
        {
            return Regex
                .Replace(typeName, @"^(?<name>.*?)(Command|Cmd)$", "${name}", RegexOptions.IgnoreCase)
                .ToLowerInvariant();
        }
    }
}