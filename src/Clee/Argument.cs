namespace Clee
{
    public class Argument
    {
        public Argument(string name, string value, bool isShortName)
        {
            Name = name;
            Value = value;
            IsShortName = isShortName;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }
        public bool IsShortName { get; private set; }

        #region equality members

        protected bool Equals(Argument other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Value, other.Value) && IsShortName == other.IsShortName;
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
                var hashCode = (Name != null
                    ? Name.GetHashCode()
                    : 0);
                hashCode = (hashCode*397) ^ (Value != null
                    ? Value.GetHashCode()
                    : 0);
                hashCode = (hashCode*397) ^ IsShortName.GetHashCode();
                return hashCode;
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

        public static Argument CreateLongNamed(string name, string value)
        {
            return new Argument(name, value, false);
        }

        public static Argument CreateShortNamed(string name, string value)
        {
            return new Argument(name, value, true);
        }
    }
}