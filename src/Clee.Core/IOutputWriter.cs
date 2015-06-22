namespace Clee
{
    public interface IOutputWriter
    {
        void WriteLine();
        void WriteLine(string text);
        void WriteLine(string format, params object[] args);
    }
}