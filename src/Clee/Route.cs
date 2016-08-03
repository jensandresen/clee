using System;

namespace Clee
{
    public class Route
    {
        private readonly CommandMetaData _commandMetaData;
        private readonly Path _path;

        public Route(CommandMetaData commandMetaData, Path path)
        {
            _commandMetaData = commandMetaData;
            _path = path;
        }

        public Type CommandType
        {
            get { return _commandMetaData.CommandType; }
        }

        public Path Path
        {
            get { return _path; }
        }

        #region equality functionality

        protected bool Equals(Route other)
        {
            return Equals(_commandMetaData, other._commandMetaData) && Equals(_path, other._path);
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