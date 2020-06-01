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
        /// Builds the resolution expression for the requested service.
        /// </summary>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <param name="typeInformation">The type info of the requested service.</param>
        /// <returns>The built expression tree.</returns>
        Expression BuildExpressionForType(ResolutionContext resolutionContext, TypeInformation typeInformation);

        /// <summary>
        /// Builds all the resolution expressions for the enumerable service request.
        /// </summary>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <param name="typeInformation">The type info of the requested service.</param>
        /// <returns>The built expression tree.</returns>
        IEnumerable<Expression> BuildExpressionsForEnumerableRequest(ResolutionContext resolutionContext, TypeInformation typeInformation);
    }
}
