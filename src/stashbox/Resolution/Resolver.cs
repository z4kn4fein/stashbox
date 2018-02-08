using Stashbox.Entity;
using System;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents a dependency resolver.
    /// </summary>
    public abstract class Resolver
    {
        /// <summary>
        /// Indicates that the resolver can be used for resolve enumerable arguments, if it's set to true the container will call the <see cref="GetExpressions" /> method.
        /// </summary>
        public virtual bool SupportsMany => false;

        /// <summary>
        /// Produces an expression for creating an instance.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <returns>The expression.</returns>
        public abstract Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext);

        /// <summary>
        /// If <see cref="SupportsMany"/> is set to true, this method will be called to collect the enumerable item expressions.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionContext">The resolution info.</param>
        /// <returns>The enumerable item expressions.</returns>
        public virtual Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            throw new NotImplementedException("The SupportsMany property is set to true, but the GetManyExpression method is not implemented.");
        }

        /// <summary>
        /// Returns true, if the resolver can be used to activate the requested service, otherwise false.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <returns>Returns true, if the resolver can be used to activate the requested service, otherwise false.</returns>
        public abstract bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext);
    }
}
