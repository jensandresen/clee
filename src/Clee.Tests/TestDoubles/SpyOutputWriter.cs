using System.Text;

namespace Clee.Tests.TestDoubles
{
    internal class SpyOutputWriter : IOutputWriter
    {
        public readonly StringBuilder Output = new StringBuilder();

        public void WriteLine()
        {
            Output.AppendLine();
        }

        public void WriteLine(string text)
        {
            Output.AppendLine(text);
        }

        public void WriteLine(string format, params object[] args)
        {
            Output.AppendFormat(format, args);
        }
    }
}