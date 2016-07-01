namespace Clee.Tests
{
    public class ReturnCode
    {
        public static readonly ReturnCode Default = new ReturnCode(CommandExecutionResultsType.Error);
        
        private readonly int _errorCode;

        public ReturnCode(int errorCode)
        {
            _errorCode = errorCode;
        }

        public ReturnCode(CommandExecutionResultsType executionResult)
        {
            _errorCode = (int) executionResult;
        }

        public int ToInt()
        {
            return _errorCode;
        }

        protected bool Equals(ReturnCode other)
        {
            return _errorCode == other._errorCode;
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
            return Equals((ReturnCode) obj);
        }

        public override int GetHashCode()
        {
            return _errorCode;
        }

        public static bool operator ==(ReturnCode left, ReturnCode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ReturnCode left, ReturnCode right)
        {
            return !Equals(left, right);
        }

        public static implicit operator int(ReturnCode returnCode)
        {
            return returnCode.ToInt();
        }

        public static implicit operator ReturnCode(int returnCode)
        {
            return new ReturnCode(returnCode);
        }
    }
}