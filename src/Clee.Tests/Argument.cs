namespace Clee.Tests
{
    public struct Argument
    {
        public Argument(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name;
        public string Value;

        public override string ToString()
        {
            return string.Format("({0}|{1})", Name, Value);
        }
    }
}