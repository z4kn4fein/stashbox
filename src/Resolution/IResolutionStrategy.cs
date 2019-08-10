using Stashbox.Entity;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Represents a resolution strategy.
    /// </summary>
    public interface IResolutionStrategy
    {
        /// <summary>
        /// Builds a resolution expression for a dependency.
        /// </summary>
        /// <param name="containerContext">The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/></param>
        /// <param name="resolutionContext">The resolution info.</param>
        /// <param name="typeInformation">The type info of the requested service.</param>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <param name="forceSkipUnknownTypeCheck">If true, the unknown type resolution will be skipped even if it's enabled by the container configuration.</param>
        /// <returns>The created resolution target.</returns>
        Expression BuildResolutionExpression(IContainerContext containerContext, ResolutionContext resolutionContext, TypeInformation typeInformation,
            IEnumerable<InjectionParameter> injectionParameters = null, bool forceSkipUnknownTypeCheck = false);

        /// <summary>
        /// Builds all the resolution expressions for an enumerable dependency.
        /// </summary>
        /// <param name="containerContext">The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/></param>
        /// <param name="resolutionContext">The resolution info.</param>
        /// <param name="typeInformation">The type info of the requested service.</param>
        /// <returns>The created resolution target.</returns>
        Expression[] BuildAllResolutionExpressions(IContainerContext containerContext, ResolutionContext resolutionContext,
            TypeInformation typeInformation);
    }
}
