using System;

namespace MetaPrograms
{
    public class InvalidCodeModelException : Exception
    {
        public InvalidCodeModelException(string message)
            : base(message)
        {
        }

        public InvalidCodeModelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
