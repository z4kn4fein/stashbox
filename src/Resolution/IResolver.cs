using Stashbox.Entity;
using Stashbox.Resolution.Resolvers;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents a dependency resolver.
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Produces an expression for creating an instance.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <returns>The expression.</returns>
        Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext);

        /// <summary>
        /// Returns true, if the resolver can be used to activate the requested service, otherwise false.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <returns>Returns true, if the resolver can be used to activate the requested service, otherwise false.</returns>
        bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext);
    }

    /// <summary>
    /// Represents a dependency resolver used by the <see cref="EnumerableResolver"/> to construct collection of expressions.
    /// </summary>
    public interface IMultiServiceResolver : IResolver
    {
        /// <summary>
        /// Produces an array of expressions, one for every registered service identified by the requested type.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionContext">The resolution info.</param>
        /// <returns>The enumerable item expressions.</returns>
        Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext);
    }
}
