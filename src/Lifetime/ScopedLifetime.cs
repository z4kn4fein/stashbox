using Stashbox.BuildUp;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a scoped lifetime.
    /// </summary>
    public class ScopedLifetime : ScopedLifetimeBase
    {
        /// <inheritdoc />
        public override ILifetime Create() => new ScopedLifetime();

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            var variable = resolutionContext.GetKnownVariableOrDefault(base.ScopeId);
            if (variable != null)
                return variable;

            var factory = base.GetFactoryExpression(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
            if (factory == null)
                return null;

            var expression = resolutionContext.CurrentScopeParameter
                .CallMethod(Constants.GetOrAddScopedItemMethod, base.ScopeId.AsConstant(), base.Sync.AsConstant(), factory)
                .ConvertTo(resolveType);

            return base.StoreExpressionIntoLocalVariable(expression, resolutionContext, resolveType);
        }
    }
}
