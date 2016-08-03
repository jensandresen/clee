using System;
using System.Reflection;

namespace Clee
{
    public class ArgumentMetaData
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly ArgumentAttribute _attribute;

        public ArgumentMetaData(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            _propertyInfo = propertyInfo;
            _attribute = propertyInfo.GetCustomAttribute<ArgumentAttribute>();

            if (_attribute == null)
            {
                throw new ArgumentException(
                    message: string.Format("Property \"{0}\" on {1} is not decorated with the attribute {2}.",
                        propertyInfo,
                        propertyInfo.DeclaringType,
                        typeof (ArgumentAttribute)),
                    paramName: "propertyInfo"
                    );
            }
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