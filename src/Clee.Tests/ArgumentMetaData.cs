using System;
using System.Reflection;

namespace Clee.Tests
{
    public class ArgumentMetaData
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly ArgumentAttribute _attribute;

        public ArgumentMetaData(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            _attribute = _propertyInfo.GetCustomAttribute<ArgumentAttribute>();
        }

        public Type ArgumentType
        {
            get { return _propertyInfo.PropertyType; }
        }

        public string ArgumentLongName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_attribute.LongName))
                {
                    return _attribute.LongName;
                }

                return _propertyInfo.Name.ToLower();
            }
        }

        public char? ArgumentShortName
        {
            get
            {
                if (_attribute.ShortName == ArgumentAttribute.EmptyChar)
                {
                    return null;
                }

                return _attribute.ShortName;
            }
        }

        public string ArgumentDescription
        {
            get
            {
                if (_attribute.Description == null)
                {
                    return "";
                }

                return _attribute.Description;
            }
        }

        public bool ArgumentIsRequired
        {
            get { return _attribute.IsRequired; }
        }
    }
}