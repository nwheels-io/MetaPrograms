using System;

namespace MetaPrograms.CodeModel.Imperative
{
    public class CodeReadErrorException : Exception
    {
        public CodeReadErrorException(string message)
            : base(message)
        {
        }

        public CodeReadErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
