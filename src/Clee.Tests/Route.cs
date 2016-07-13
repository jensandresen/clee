using System;

namespace Clee.Tests
{
    public class Route
    {
        private readonly Type _commandType;
        private readonly string _name;

        public Route(Type commandType, string name)
        {
            _commandType = commandType;
            _name = name;
        }

        public Type CommandType
        {
            get { return _commandType; }
        }

        public string Name
        {
            get { return _name; }
        }

        #region equality functionality

        protected bool Equals(Route other)
        {
            return Equals(_commandType, other._commandType) && string.Equals(_name, other._name);
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
            return Equals((Route) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_commandType != null
                    ? _commandType.GetHashCode()
                    : 0)*397) ^ (_name != null
                        ? _name.GetHashCode()
                        : 0);
            }
        }

        public static bool operator ==(Route left, Route right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Route left, Route right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}