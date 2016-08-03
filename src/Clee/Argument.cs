namespace Clee
{
    public class Argument
    {
        public Argument(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }

        #region equality members

        protected bool Equals(Argument other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Argument) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null
                    ? Name.GetHashCode()
                    : 0)*397) ^ (Value != null
                        ? Value.GetHashCode()
                        : 0);
            }
        }

        public static bool operator ==(Argument left, Argument right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Argument left, Argument right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}