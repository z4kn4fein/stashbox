using System;

namespace Stashbox.Exceptions
{
    public class ExceptionBase : Exception
    {
        public string TypeName { get; set; }

        public ExceptionBase(string typeName, string message = null, Exception innerException = null)
            : base(message, innerException)
        {
            this.TypeName = typeName;
        }
    }
}