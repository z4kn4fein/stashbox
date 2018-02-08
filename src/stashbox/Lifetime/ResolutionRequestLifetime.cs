using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a per resolution request lifetime.
    /// </summary>
    public class ResolutionRequestLifetime : ScopedLifetimeBase
    {
        /// <inheritdoc />
        public override ILifetime Create() => new ResolutionRequestLifetime();

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration,
            IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            var variable = resolutionContext.GetKnownVariableOrDefault(base.ScopeId);
            if (variable != null)
                return variable;

            var expression = base.GetExpression(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);

            return expression == null ? null : base.StoreExpressionIntoLocalVariable(expression, resolutionContext, resolveType);
        }
    }
}
