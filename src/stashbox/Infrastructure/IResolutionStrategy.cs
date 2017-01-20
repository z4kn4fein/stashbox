using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a resolution strategy.
    /// </summary>
    public interface IResolutionStrategy
    {
        /// <summary>
        /// Checks whether a type can be resolved or not.
        /// </summary>
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <param name="containerContext">The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/></param>
        /// <param name="typeInformation">The type info of the requested service.</param>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>True if the type can be resolved, otherwise false.</returns>
        bool CanResolve(ResolutionInfo resolutionInfo, IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters);

        /// <summary>
        /// Builds a <see cref="ResolutionTarget"/> for a dependency.
        /// </summary>
        /// <param name="containerContext">The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/></param>
        /// <param name="typeInformation">The type info of the requested service.</param>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The created resolution target.</returns>
        ResolutionTarget BuildResolutionTarget(IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters);

        /// <summary>
        /// Evaluates a <see cref="ResolutionTarget"/>
        /// </summary>
        /// <param name="resolutionTarget">The resolution target object.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <returns>The evaluated instance.</returns>
        object EvaluateResolutionTarget(ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo);

        /// <summary>
        /// Gets an expression for evaluating a <see cref="ResolutionTarget"/>
        /// </summary>
        /// <param name="resolutionTarget">The resolution target object.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolutionInfoExpression">The expression of the info about the actual resolution.</param>
        /// <returns>The expression.</returns>
        Expression GetExpressionForResolutionTarget(ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression);
    }
}
