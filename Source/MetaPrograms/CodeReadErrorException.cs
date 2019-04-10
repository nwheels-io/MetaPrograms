using System;
using System.Runtime.Serialization;

namespace MetaPrograms
{
    [Serializable]
    public class CodeReadErrorException : Exception
    {
        public CodeReadErrorException()
        {
        }
        
        public CodeReadErrorException(string message)
            : base(message)
        {
        }

        public CodeReadErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CodeReadErrorException(
            SerializationInfo info,
            StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
