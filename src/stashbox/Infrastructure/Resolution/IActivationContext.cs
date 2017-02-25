using System;

namespace Stashbox.Infrastructure.Resolution
{
    /// <summary>
    /// Represents an activation context, can be used for type activation.
    /// </summary>
    public interface IActivationContext
    {
        /// <summary>
        /// Activates a type.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="name">The service name.</param>
        /// <returns>The resolved object.</returns>
        object Activate(Type type, string name = null);

        /// <summary>
        /// Activates a type via a delegate.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <param name="name">The service name.</param>
        /// <returns>The delegate which can be used for activate a type.</returns>
        Delegate ActivateFactory(Type type, Type[] parameterTypes, string name = null);
    }
}
