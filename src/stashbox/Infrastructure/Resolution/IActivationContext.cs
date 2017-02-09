using System;
using System.Collections.Generic;
using Stashbox.Entity;

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
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>The resolved object.</returns>
        object Activate(ResolutionInfo resolutionInfo, TypeInformation typeInfo);

        /// <summary>
        /// Activates a type via a delegate.
        /// </summary>
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>The delegate which can be used for activate a type.</returns>
        Delegate ActivateFactory(ResolutionInfo resolutionInfo, TypeInformation typeInfo, Type[] parameterTypes);
    }
}
