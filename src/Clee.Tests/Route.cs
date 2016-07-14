using System;

namespace Clee.Tests
{
    public class Route
    {
        private readonly CommandMetaData _commandMetaData;
        private readonly string _path;

        public Route(Type commandType, string path)
        {
            _commandMetaData = new CommandMetaData(commandType);
            _path = path;
        }

        public Type CommandType
        {
            get { return _commandMetaData.CommandType; }
        }

        public string Path
        {
            get { return _path; }
        }

        #region equality functionality

        protected bool Equals(Route other)
        {
            return Equals(_commandMetaData, other._commandMetaData) && string.Equals(_path, other._path);
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
                return ((_commandMetaData != null
                    ? _commandMetaData.GetHashCode()
                    : 0)*397) ^ (_path != null
                        ? _path.GetHashCode()
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