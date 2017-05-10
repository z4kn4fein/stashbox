using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure.Resolution
{
    /// <summary>
    /// Represents the resolver selector.
    /// </summary>
    public interface IResolverSelector
    {
        /// <summary>
        /// True if a type is resolvable by any of the stored resolvers, otherwise false.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <returns></returns>
        bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo);

        /// <summary>
        /// Gets an expression built by a selected <see cref="Resolver"/>.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <returns>The expression.</returns>
        Expression GetResolverExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo);

        /// <summary>
        /// Gets the expressions built by a selected <see cref="Resolver"/>.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <returns>The expressions.</returns>
        Expression[] GetResolverExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo);

        /// <summary>
        /// Adds a resolver to the selector.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        void AddResolver(Resolver resolver);
    }
}
