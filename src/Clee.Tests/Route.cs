using System;

namespace Clee.Tests
{
    public class Route
    {
        private readonly Type _commandType;

        public Route(Type commandType)
        {
            _commandType = commandType;
        }

        public Type CommandType
        {
            get { return _commandType; }
        }

        #region equality functionality

        protected bool Equals(Route other)
        {
            return Equals(_commandType, other._commandType);
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
            return (_commandType != null
                ? _commandType.GetHashCode()
                : 0);
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