using System.Collections.Generic;

namespace Stashbox.Resolution;

/// <summary>
/// The base interface for wrappers and resolvers.
/// </summary>
public interface IResolver { }
    
/// <summary>
/// Represents a dependency resolver.
/// </summary>
public interface IServiceResolver : IResolver
{
    /// <summary>
    /// Produces an expression for creating an instance.
    /// </summary>
    /// <param name="resolutionStrategy">The resolution strategy used to build the underlying resolution expression tree.</param>
    /// <param name="typeInfo">The information about the type to resolve.</param>
    /// <param name="resolutionContext">The contextual information about the current resolution call.</param>
    /// <returns>The built resolution expression.</returns>
    ServiceContext GetExpression(
        IResolutionStrategy resolutionStrategy,
        TypeInformation typeInfo,
        ResolutionContext resolutionContext);

    /// <summary>
    /// Returns true, if the resolver can be used to activate the requested service, otherwise false.
    /// </summary>
    /// <param name="typeInfo">The information about the type to resolve.</param>
    /// <param name="resolutionContext">The contextual information about the current resolution call.</param>
    /// <returns>Returns true, if the resolver can be used to activate the requested service, otherwise false.</returns>
    bool CanUseForResolution(TypeInformation typeInfo,
        ResolutionContext resolutionContext);
}

/// <summary>
/// Represents a dependency resolver that can produce a collection of services.
/// </summary>
public interface IEnumerableSupportedResolver : IServiceResolver
{
    /// <summary>
    /// Produces an array of expressions, one for every registered service identified by the requested type.
    /// </summary>
    /// <param name="resolutionStrategy">The resolution strategy used to build the underlying resolution expression tree.</param>
    /// <param name="typeInfo">The information about the type to resolve.</param>
    /// <param name="resolutionContext">The contextual information about the current resolution call.</param>
    /// <returns>The array of all the resolution expression built by the resolver.</returns>
    IEnumerable<ServiceContext> GetExpressionsForEnumerableRequest(
        IResolutionStrategy resolutionStrategy,
        TypeInformation typeInfo,
        ResolutionContext resolutionContext);
}