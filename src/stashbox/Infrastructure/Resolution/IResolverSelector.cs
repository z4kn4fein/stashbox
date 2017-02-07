using Stashbox.Entity;

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
        /// <returns></returns>
        bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo);

        /// <summary>
        /// Tries to get a resolver for a given type.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="resolver">The selected resolver.</param>
        /// <returns>True if a resolver is selected.</returns>
        bool TryChooseResolver(IContainerContext containerContext, TypeInformation typeInfo, out Resolver resolver);

        /// <summary>
        /// Adds a resolver to the selector.
        /// </summary>
        /// <param name="resolverRegistration">The resolver registration.</param>
        void AddResolver(ResolverRegistration resolverRegistration);

        /// <summary>
        /// Creates a copy from the selector.
        /// </summary>
        /// <returns>The copied selector.</returns>
        IResolverSelector CreateCopy();
    }
}
