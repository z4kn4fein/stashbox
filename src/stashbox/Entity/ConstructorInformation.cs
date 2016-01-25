using Stashbox.Attributes;
using System.Reflection;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents information about a constructor.
    /// </summary>
    public class ConstructorInformation
    {
        /// <summary>
        /// Stores the reflected constructor info.
        /// </summary>
        public ConstructorInfo Constructor { get; set; }

        /// <summary>
        /// True if the constructor is decorated with <see cref="InjectionConstructorAttribute"/>
        /// </summary>
        public bool HasInjectionAttribute { get; set; }

        /// <summary>
        /// The parameters of the constructor.
        /// </summary>
        public TypeInformation[] Parameters { get; set; }
    }
}