using Stashbox.Registration;
using System.Collections.Generic;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents a resolution strategy.
    /// </summary>
    public interface IResolutionStrategy
    {
        /// <summary>
        /// Registers an <see cref="IResolver"/>.
        /// </summary>
        /// <param name="resolver">The resolver implementation.</param>
        void RegisterResolver(IResolver resolver);

        /// <summary>
        /// Builds the resolution expression for the requested service.
        /// </summary>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <param name="typeInformation">The type info of the requested service.</param>
        /// <returns>The built expression tree.</returns>
        ServiceContext BuildExpressionForType(ResolutionContext resolutionContext, TypeInformation typeInformation);

        /// <summary>
        /// Builds all the resolution expressions for the enumerable service request.
        /// </summary>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <param name="typeInformation">The type info of the requested service.</param>
        /// <returns>The built expression tree.</returns>
        IEnumerable<ServiceContext> BuildExpressionsForEnumerableRequest(ResolutionContext resolutionContext, TypeInformation typeInformation);

        /// <summary>
        /// Builds the resolution expression for the requested service registration.
        /// </summary>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <param name="typeInformation">The type info of the requested service.</param>
        /// <returns>The built expression tree.</returns>
        ServiceContext BuildExpressionForRegistration(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, TypeInformation typeInformation);

        /// <summary>
        /// Determines whether a type is resolvable with the current container state or not.
        /// </summary>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <param name="typeInformation">The type info of the requested service.</param>
        /// <returns>True if a type is resolvable, otherwise false.</returns>
        bool IsTypeResolvable(ResolutionContext resolutionContext, TypeInformation typeInformation);
    }
}
