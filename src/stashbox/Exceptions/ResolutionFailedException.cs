using System;

namespace Stashbox.Exceptions
{
    public class ResolutionFailedException : ExceptionBase
    {
        public ResolutionFailedException(string typeName, string message = null, Exception innerException = null)
            : base(typeName, string.Concat(message, " ", typeName), innerException)
        { }
    }
}