using System;

namespace Stashbox.Exceptions
{
    public class RegistrationNotFoundException : ExceptionBase
    {
        public RegistrationNotFoundException(string typeName, string message = null, Exception innerException = null)
            : base(typeName, string.Concat(message, " ", typeName), innerException)
        { }
    }
}