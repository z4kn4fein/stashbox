using Stashbox.Entity;
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
        /// <param name="resolutionScope">The resolution scope.</param>
        /// <param name="name">The service name.</param>
        /// <returns>The resolved object.</returns>
        object Activate(Type type, IResolutionScope resolutionScope, string name = null);

        /// <summary>
        /// Activates a type.
        /// </summary>
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <param name="type">The type.</param>
        /// <param name="name">The service name.</param>
        /// <returns>The resolved object.</returns>
        object Activate(ResolutionInfo resolutionInfo, Type type, string name = null);

        /// <summary>
        /// Activates a type via a delegate.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <param name="resolutionScope">The resolution scope.</param>
        /// <param name="name">The service name.</param>
        /// <returns>The delegate which can be used for activate a type.</returns>
        Delegate ActivateFactory(Type type, Type[] parameterTypes, IResolutionScope resolutionScope, string name = null);
    }
}
