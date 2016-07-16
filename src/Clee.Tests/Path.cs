using System;
using System.Collections.Generic;

namespace Clee.Tests
{
    public class Path
    {
        private readonly string _rootValue;
        private readonly LinkedList<string> _segments = new LinkedList<string>();

        public Path(string rootValue)
        {
            if (string.IsNullOrWhiteSpace(rootValue))
            {
                throw new ArgumentException(string.Format("Invalid value \"{0}\" used to initialize a path instance.", rootValue), "rootValue");
            }

            if (rootValue.Contains(" "))
            {
                throw new ArgumentException("Path cannot be initialized with a value containing spaces.", "rootValue");
            }

            _rootValue = rootValue;
        }

        public override string ToString()
        {
            var result = "/" + _rootValue;
            
            if (_segments.Count == 0)
            {
                return result;
            }

            return result + "/" + string.Join("/", _segments);
        }

        public void AddSegment(string segmentValue)
        {
            _segments.AddLast(segmentValue);
        }

        #region equality members

        protected bool Equals(Path other)
        {
            return string.Equals(this.ToString(), other.ToString());
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
            return Equals((Path) obj);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static bool operator ==(Path left, Path right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Path left, Path right)
        {
            return !Equals(left, right);
        }

        #endregion

        public static Path Parse(string text)
        {
            if (text == null)
            {
                throw new ArgumentException("Cannot parse null to a valid Path.", "text");
            }

            var segments = text
                .Replace(' ', '/')
                .Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
            {
                throw new ArgumentException(string.Format("Parsing input of \"{0}\" yields empty or invalid path.", text));
            }

            var result = new Path(segments[0]);

            for (var i = 1; i < segments.Length; i++)
            {
                result.AddSegment(segments[i]);
            }

            return result;
        }

        public static bool TryParse(string text, out Path instance)
        {
            instance = null;

            try
            {
                instance = Parse(text);
                return true;
            }
            catch
            {
                // ignored
            }

            return false;
        }
    }
}