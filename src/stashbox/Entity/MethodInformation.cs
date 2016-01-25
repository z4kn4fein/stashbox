using System.Reflection;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents information about a method.
    /// </summary>
    public class MethodInformation
    {
        /// <summary>
        /// Stores the reflected method information.
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Stores the parameters of the method.
        /// </summary>
        public TypeInformation[] Parameters { get; set; }
    }
}
