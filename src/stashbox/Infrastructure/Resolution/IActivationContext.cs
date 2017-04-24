using System;
using Stashbox.Exceptions;

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
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The resolved object.</returns>
        object Activate(Type type, IResolutionScope resolutionScope, object name = null, bool nullResultAllowed = false);

        /// <summary>
        /// Activates a type via a delegate.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <param name="resolutionScope">The resolution scope.</param>
        /// <param name="name">The service name.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The delegate which can be used for activate a type.</returns>
        Delegate ActivateFactory(Type type, Type[] parameterTypes, IResolutionScope resolutionScope, object name = null, bool nullResultAllowed = false);
    }
}
